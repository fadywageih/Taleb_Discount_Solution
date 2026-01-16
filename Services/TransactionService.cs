namespace Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IProductRepository _productRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IProductRepository productRepository,
            IVendorRepository vendorRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository ;
            _productRepository = productRepository ;
            _vendorRepository = vendorRepository ;
            _userRepository = userRepository ;
            _httpContextAccessor = httpContextAccessor ;
            _mapper = mapper;
            _logger = logger;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("User not authenticated");
            return userId;
        }
        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");
            return user;
        }
        public async Task<TransactionDto> CreateTransactionAsync(TransactionCreateDto dto)
        {
            if (!dto.Validate())
                throw new ArgumentException("Invalid transaction data");
            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product == null || !product.IsActive)
                throw new InvalidOperationException($"Product not found or inactive");

            if (product.Quantity < dto.Quantity)
                throw new InvalidOperationException($"Insufficient quantity. Available: {product.Quantity}");
            var vendor = await _vendorRepository.GetVendorByIdAsync(product.VendorId);
            if (vendor == null)
                throw new InvalidOperationException($"Vendor not found");
            var customer = await GetCurrentUserAsync();
            decimal finalPrice = product.DiscountPrice ?? product.Price;
            if (!string.IsNullOrEmpty(dto.DiscountCode))
            {
                finalPrice = ApplyDiscount(finalPrice, dto.DiscountCode);
            }
            var transactionNumber = await _transactionRepository.GenerateUniqueTransactionNumberAsync();
            var transaction = new Transaction
            {
                TransactionNumber = transactionNumber,
                DiscountCode = dto.DiscountCode,
                ProductId = product.Id,
                ProductName = product.Name,
                ProductPictureUrl = product.PictureUrl,
                VendorId = vendor.Id,
                VendorName = vendor.BusinessName,
                CustomerId = customer.Id,
                CustomerName = GetCustomerDisplayName(customer), 
                CustomerEmail = customer.Email,
                TransactionDate = DateTime.UtcNow,
                Price = finalPrice,
                Quantity = dto.Quantity,
                Status = TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            product.Quantity -= dto.Quantity;
            await _productRepository.UpdateAsync(product);
            var createdTransaction = await _transactionRepository.AddAsync(transaction);
            _logger.LogInformation($"Transaction {transactionNumber} created for customer {customer.Email}");
            return await GetTransactionByIdAsync(createdTransaction.Id);
        }
        public async Task<TransactionDto> GetTransactionByIdAsync(Guid id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
                throw new InvalidOperationException($"Transaction not found");
            return _mapper.Map<TransactionDto>(transaction);
        }
        public async Task<IEnumerable<TransactionDto>> GetVendorTransactionsAsync(Guid vendorId)
        {
            var transactions = await _transactionRepository.GetVendorTransactionsAsync(vendorId);
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }
        public async Task<IEnumerable<TransactionDto>> GetCustomerTransactionsAsync(Guid customerId)
        {
            var transactions = await _transactionRepository.GetCustomerTransactionsAsync(customerId);
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }
        public async Task<TransactionDto> AcceptTransactionAsync(Guid transactionId)
        {
            var userId = GetCurrentUserId();
            var vendor = await _vendorRepository.GetVendorByUserIdAsync(userId);
            if (vendor == null)
            {
                throw new InvalidOperationException($"Vendor profile not found. Please complete your vendor profile setup.");
            }
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                throw new InvalidOperationException($"Transaction with ID {transactionId} not found.");
            }
            if (transaction.VendorId != vendor.Id)
            {
                var actualVendor = await _vendorRepository.GetVendorByIdAsync(transaction.VendorId);
                var actualVendorName = actualVendor?.BusinessName ?? "Unknown Vendor";

                throw new UnauthorizedAccessException(
                    $"You cannot accept this transaction. It belongs to '{actualVendorName}', " +
                    $"but you are logged in as '{vendor.BusinessName}'. " +
                    $"Please login with the correct vendor account.");
            }
            if (transaction.Status != TransactionStatus.Pending)
            {
                throw new InvalidOperationException(
                    $"Transaction cannot be accepted. Current status is '{transaction.Status}'. " +
                    $"Only 'Pending' transactions can be accepted.");
            }

            var success = await _transactionRepository.AcceptTransactionAsync(transactionId, vendor.Id);

            if (!success)
            {
                throw new InvalidOperationException($"Failed to update transaction status.");
            }
            return await GetTransactionByIdAsync(transactionId);
        }
        public async Task<TransactionDto> RejectTransactionAsync(Guid transactionId, string reason)
        {
            try
            {
                _logger.LogInformation($"🔄 Attempting to reject transaction: {transactionId}");
                var userId = GetCurrentUserId();
                _logger.LogInformation($"👤 Current UserId: {userId}");
                var vendor = await _vendorRepository.GetVendorByUserIdAsync(userId);
                if (vendor == null)
                {
                    _logger.LogError($"❌ No vendor found for user: {userId}");
                    throw new InvalidOperationException($"Vendor profile not found. Please complete your vendor profile setup.");
                }

                _logger.LogInformation($"🏢 Vendor found - ID: {vendor.Id}, Business: {vendor.BusinessName}");
                var transaction = await _transactionRepository.GetByIdAsync(transactionId);
                if (transaction == null)
                {
                    _logger.LogError($"❌ Transaction not found: {transactionId}");
                    throw new InvalidOperationException($"Transaction with ID {transactionId} not found.");
                }

                _logger.LogInformation($"📦 Transaction details - Number: {transaction.TransactionNumber}, " +
                                      $"Status: {transaction.Status}, VendorId: {transaction.VendorId}, " +
                                      $"Product: {transaction.ProductName}");
                if (transaction.VendorId != vendor.Id)
                {
                    _logger.LogError($"🚫 Authorization failed - Transaction belongs to Vendor: {transaction.VendorId}, " +
                                   $"Current vendor: {vendor.Id}");

                    var actualVendor = await _vendorRepository.GetVendorByIdAsync(transaction.VendorId);
                    var actualVendorName = actualVendor?.BusinessName ?? "Unknown Vendor";

                    throw new UnauthorizedAccessException(
                        $"You cannot reject this transaction. It belongs to '{actualVendorName}', " +
                        $"but you are logged in as '{vendor.BusinessName}'.");
                }
                if (transaction.Status != TransactionStatus.Pending)
                {
                    _logger.LogError($"⚠️ Invalid transaction status - Current: {transaction.Status}, Required: Pending");
                    throw new InvalidOperationException(
                        $"Transaction cannot be rejected. Current status is '{transaction.Status}'. " +
                        $"Only 'Pending' transactions can be rejected.");
                }
                _logger.LogInformation($"✅ All checks passed. Rejecting transaction...");
                var success = await _transactionRepository.RejectTransactionAsync(transactionId, vendor.Id, reason);

                if (!success)
                {
                    _logger.LogError($"❌ Failed to reject transaction in repository: {transactionId}");
                    throw new InvalidOperationException($"Failed to update transaction status.");
                }
                var product = await _productRepository.GetByIdAsync(transaction.ProductId);
                if (product != null)
                {
                    product.Quantity += transaction.Quantity;
                    await _productRepository.UpdateAsync(product);
                    _logger.LogInformation($"📦 Product quantity restored: {product.Name} (+{transaction.Quantity})");
                }
                var updatedTransaction = await GetTransactionByIdAsync(transactionId);
                _logger.LogInformation($"🎉 Transaction {updatedTransaction.TransactionNumber} " +
                                      $"rejected successfully by vendor {vendor.BusinessName}");

                return updatedTransaction;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Authorization error rejecting transaction: {transactionId}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, $"Invalid operation rejecting transaction: {transactionId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error rejecting transaction: {transactionId}");
                throw new InvalidOperationException($"An unexpected error occurred while rejecting the transaction.");
            }
        }
        public async Task<TransactionStatsDto> GetVendorStatsAsync(Guid vendorId)
        {
            var (totalEarnings, totalOrders) = await _transactionRepository.GetVendorStatsAsync(vendorId);
            var transactions = await _transactionRepository.GetVendorTransactionsAsync(vendorId);

            var stats = new TransactionStatsDto
            {
                TotalEarnings = totalEarnings,
                TotalOrders = totalOrders,
                PendingOrders = transactions.Count(t => t.Status == TransactionStatus.Pending),
                AcceptedOrders = transactions.Count(t => t.Status == TransactionStatus.Accepted),
                CompletedOrders = transactions.Count(t => t.Status == TransactionStatus.Completed),
                RejectedOrders = transactions.Count(t => t.Status == TransactionStatus.Rejected)
            };
            return stats;
        }
        private decimal ApplyDiscount(decimal price, string discountCode)
        {
            return price * 0.9m;
        }
        public async Task<TransactionDto> CancelTransactionAsync(Guid transactionId)
        {
            var customerId = GetCurrentUserId();
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new InvalidOperationException("Transaction not found");
            if (transaction.CustomerId != customerId)
                throw new UnauthorizedAccessException("You are not authorized to cancel this transaction");
            if (transaction.Status != TransactionStatus.Pending)
                throw new InvalidOperationException($"Cannot cancel transaction with status: {transaction.Status}");
            transaction.Status = TransactionStatus.Rejected;
            transaction.RejectionReason = "Cancelled by customer";
            transaction.RejectedDate = DateTime.UtcNow;
            transaction.UpdatedAt = DateTime.UtcNow;
            await _transactionRepository.UpdateAsync(transaction);
            var product = await _productRepository.GetByIdAsync(transaction.ProductId);
            if (product != null)
            {
                product.Quantity += transaction.Quantity;
                await _productRepository.UpdateAsync(product);
            }
            _logger.LogInformation($"Transaction {transaction.TransactionNumber} cancelled by customer {customerId}");
            return await GetTransactionByIdAsync(transactionId);
        }
        private string GetCustomerDisplayName(ApplicationUser user)
        {
            if (!string.IsNullOrEmpty(user.Name))
                return user.Name;
            var emailParts = user.Email?.Split('@') ?? Array.Empty<string>();
            return emailParts.Length > 0 ? emailParts[0] : user.Email ?? string.Empty;
        }
    }
}