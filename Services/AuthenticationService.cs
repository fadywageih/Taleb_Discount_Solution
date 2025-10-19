using Domain.Contracts;

namespace Services
{
    public class AuthenticationService(UserManager<ApplicationUser> _userManager, IMapper _mapper
        , IOptions<JwtOptions> options, IAttachmentService _attachmentService,IUnitOfWork _unitOfWork) : IAuthenticationService
    {
        public async Task<bool> CheckIfEmailExist(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
        public async Task<UserResultDto> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new UserNotFoundException(email);
            return new UserResultDto(DisplayName: user.Name, Email: user.Email, Token: await CreateTokenAsync(user)
 );
        }
        public async Task<UserResultDto> RegisterSchool(SchoolRegisterDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
            {
                throw new Domain.Exceptions.ValidationException(new[] { "Email is already registered." });
            }
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                Age = dto.Age,
                NationalId = dto.NationalId,
                PhoneNumber = dto.Phone,
                UserType = "School"
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new Domain.Exceptions.ValidationException(errors);

            }
            var filePath = await _attachmentService.UploadFileAsync(dto.BirthCertificateFile, "school");
            var schoolStudent = new SchoolStudent
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Address = dto.Address,
                SchoolName = dto.SchoolName,
                Level = dto.Level,
                BirthCertificatePath = filePath
            };
            var schoolRepo = _unitOfWork.GetRepository<SchoolStudent, Guid>();
            await schoolRepo.AddAsync(schoolStudent);
            await _unitOfWork.SaveChangesAsync(); // ← هذا السطر ضروري جدًا

            return new UserResultDto(
                DisplayName: user.Name,
                Email: user.Email,
                Token: await CreateTokenAsync(user)
            );
        }

        public Task<UserResultDto> RegisterUniversity(UniversityRegisterDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<UserResultDto> RegisterVendor(VendorRegisterDto dto)
        {
            throw new NotImplementedException();
        }

        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var JwtOptions = options.Value;
            //Private Claims
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.Email,user.Email)
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claim.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: JwtOptions.Issuer,
                audience: JwtOptions.Audience,
                claims: claim,
                expires: DateTime.Now.AddDays(JwtOptions.ExpirationInDays), //الوقت اللي بتقعه
                signingCredentials: creds

                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
