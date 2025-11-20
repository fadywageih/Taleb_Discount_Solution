using Domain.Contracts;

namespace Services
{
    public class AuthenticationService(UserManager<ApplicationUser> _userManager, IMapper _mapper
        , IOptions<JwtOptions> options, IAttachmentService _attachmentService,IUnitOfWork _unitOfWork,IEmailService _emailService) : IAuthenticationService
    {
        public async Task<bool> CheckIfEmailExist(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
        public async Task<UserResultDto> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new UserNotFoundException(email);
            return new UserResultDto(DisplayName: user.Name, Email: user.Email, Token: await CreateTokenAsync(user),null);
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
            await _unitOfWork.SaveChangesAsync();

            return new UserResultDto(
                DisplayName: user.Name,
                Email: user.Email,
                Token: await CreateTokenAsync(user),
                UserType: user.UserType // إضافة هذا
            );
        }
        public async Task<UserResultDto> RegisterUniversity(UniversityRegisterDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                throw new Domain.Exceptions.ValidationException(new[] { "Email is already registered." });
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                Age = dto.Age,
                NationalId = dto.NationalId,
                PhoneNumber = dto.Phone,
                UserType = "University"
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new Domain.Exceptions.ValidationException(errors);
            }

            if (dto.NationalIdFile == null)
                throw new Domain.Exceptions.ValidationException(new[] { "National ID image is required." });

            var filePath = await _attachmentService.UploadFileAsync(dto.NationalIdFile, "university");

            var uniStudent = new UniversityStudent
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                UniversityName = dto.UniversityName,
                Faculty = dto.Faculty,
                UniversityEmail = dto.UniversityEmail,
                Level = dto.Level,
                NationalIdImagePath = filePath
            };

            var uniRepo = _unitOfWork.GetRepository<UniversityStudent, Guid>();
            await uniRepo.AddAsync(uniStudent);
            await _unitOfWork.SaveChangesAsync();

            return new UserResultDto(
                DisplayName: user.Name,
                Email: user.Email,
                Token: await CreateTokenAsync(user),
                UserType: user.UserType // إضافة هذا
            );
        }
        public async Task<UserResultDto> RegisterVendor(VendorRegisterDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                throw new Domain.Exceptions.ValidationException(new[] { "Email is already registered." });
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.Phone,
                Name = dto.BusinessName,
                UserType = "Vendor"
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new Domain.Exceptions.ValidationException(errors);
            }

            var vendor = new Vendor
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                BusinessName = dto.BusinessName,
                Description = dto.Description,
                Address = dto.Address,
                Address2 = dto.Address2,
                Website = dto.Website,
                FacebookUrl = dto.FacebookUrl
            };

            var vendorRepo = _unitOfWork.GetRepository<Vendor, Guid>();
            await vendorRepo.AddAsync(vendor);
            await _unitOfWork.SaveChangesAsync();

            return new UserResultDto(
                DisplayName: dto.BusinessName,
                Email: user.Email,
                Token: await CreateTokenAsync(user),
                UserType: user.UserType // إضافة هذا
            );
        }
        public async Task<UserResultDto> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) throw new UnauthorizedException();

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result) throw new UnauthorizedException();

            return new UserResultDto(
                DisplayName: user.Name ?? user.Email,
                Email: user.Email,
                Token: await CreateTokenAsync(user),
                UserType: user.UserType 
            );
        }
        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var JwtOptions = options.Value;
            var claim = new List<Claim>
            {new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
             new Claim(ClaimTypes.Name, user.Name),
             new Claim(ClaimTypes.Email, user.Email),
             new Claim("UserType", user.UserType),
            new Claim(ClaimTypes.Role, user.UserType)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: JwtOptions.Issuer,
                audience: JwtOptions.Audience,
                claims: claim,
                expires: DateTime.Now.AddDays(JwtOptions.ExpirationInDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<bool> SendResetPasswordEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return true; 
            }
            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"http://localhost:4200/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
                var emailMessage = new EmailDto
                {
                    To = user.Email,
                    Subject = "Reset your Password",
                    Body = $"Please reset your password by clicking here: {resetLink}"
                };
                await _emailService.SendEmailAsync(emailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> ResetPassword(string email, string token, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.ResetPasswordAsync(user, token, password);
            return result.Succeeded;
        }
    }
}
