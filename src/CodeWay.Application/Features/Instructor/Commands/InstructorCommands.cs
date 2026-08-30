namespace CodeWay.Application.Features.Instructor.Commands;

using AutoMapper;
using CodeWay.Application.Contracts;
using CodeWay.Application.Features.Instructor.DTOs;
using CodeWay.Domain.Entities.Instructor;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record CreateInstructorProfileCommand(CreateInstructorProfileDto Dto) : IRequest<InstructorProfileDto>;

public sealed record UpdateInstructorProfileCommand(Guid Id, UpdateInstructorProfileDto Dto) : IRequest<InstructorProfileDto>;

public sealed record ApproveInstructorProfileCommand(Guid Id) : IRequest<InstructorProfileDto>;

public sealed record GetInstructorProfilesQuery(bool? ApprovedOnly = null) : IRequest<IReadOnlyList<InstructorProfileDto>>;

public sealed record GetInstructorProfileByIdQuery(Guid Id) : IRequest<InstructorProfileDto>;

public sealed record GetMyInstructorProfileQuery : IRequest<InstructorProfileDto>;

public sealed record GetInstructorWalletQuery(Guid? InstructorId = null) : IRequest<InstructorWalletDto>;

public sealed record CreatePayoutRequestCommand(CreatePayoutRequestDto Dto) : IRequest<PayoutRequestDto>;

public sealed record ProcessPayoutRequestCommand(Guid Id, ProcessPayoutRequestDto Dto) : IRequest<PayoutRequestDto>;

public sealed record GetPayoutRequestsQuery(Guid? InstructorId = null) : IRequest<IReadOnlyList<PayoutRequestDto>>;

public sealed class InstructorCommandHandler :
    IRequestHandler<CreateInstructorProfileCommand, InstructorProfileDto>,
    IRequestHandler<UpdateInstructorProfileCommand, InstructorProfileDto>,
    IRequestHandler<ApproveInstructorProfileCommand, InstructorProfileDto>,
    IRequestHandler<GetInstructorProfilesQuery, IReadOnlyList<InstructorProfileDto>>,
    IRequestHandler<GetInstructorProfileByIdQuery, InstructorProfileDto>,
    IRequestHandler<GetMyInstructorProfileQuery, InstructorProfileDto>,
    IRequestHandler<GetInstructorWalletQuery, InstructorWalletDto>,
    IRequestHandler<CreatePayoutRequestCommand, PayoutRequestDto>,
    IRequestHandler<ProcessPayoutRequestCommand, PayoutRequestDto>,
    IRequestHandler<GetPayoutRequestsQuery, IReadOnlyList<PayoutRequestDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public InstructorCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<InstructorProfileDto> Handle(CreateInstructorProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required to create instructor profile.");

        var exists = await _unitOfWork.InstructorProfiles.ExistsAsync(i => i.UserId == userId, cancellationToken);
        if (exists)
            throw new ConflictException("InstructorProfile", "userId", userId);

        var profile = new InstructorProfile
        {
            UserId = userId,
            Headline = request.Dto.Headline.Trim(),
            Biography = request.Dto.Biography.Trim(),
            WebsiteUrl = request.Dto.WebsiteUrl?.Trim(),
            TwitterUrl = request.Dto.TwitterUrl?.Trim(),
            LinkedInUrl = request.Dto.LinkedInUrl?.Trim(),
            YouTubeUrl = request.Dto.YouTubeUrl?.Trim(),
            PayoutEmail = request.Dto.PayoutEmail.Trim().ToLowerInvariant(),
            IsApproved = false
        };

        await _unitOfWork.InstructorProfiles.AddAsync(profile, cancellationToken);

        // Create associated InstructorWallet
        var wallet = new InstructorWallet
        {
            InstructorId = profile.Id,
            Balance = 0,
            PendingBalance = 0,
            TotalEarned = 0
        };
        await _unitOfWork.Wallets.AddAsync(wallet, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<InstructorProfileDto>(profile);
    }

    public async Task<InstructorProfileDto> Handle(UpdateInstructorProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _unitOfWork.InstructorProfiles.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("InstructorProfile", request.Id);

        profile.Headline = request.Dto.Headline.Trim();
        profile.Biography = request.Dto.Biography.Trim();
        profile.WebsiteUrl = request.Dto.WebsiteUrl?.Trim();
        profile.TwitterUrl = request.Dto.TwitterUrl?.Trim();
        profile.LinkedInUrl = request.Dto.LinkedInUrl?.Trim();
        profile.YouTubeUrl = request.Dto.YouTubeUrl?.Trim();
        profile.PayoutEmail = request.Dto.PayoutEmail.Trim().ToLowerInvariant();

        _unitOfWork.InstructorProfiles.Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<InstructorProfileDto>(profile);
    }

    public async Task<InstructorProfileDto> Handle(ApproveInstructorProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _unitOfWork.InstructorProfiles.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("InstructorProfile", request.Id);

        profile.IsApproved = true;
        _unitOfWork.InstructorProfiles.Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<InstructorProfileDto>(profile);
    }

    public async Task<IReadOnlyList<InstructorProfileDto>> Handle(GetInstructorProfilesQuery request, CancellationToken cancellationToken)
    {
        var profiles = await _unitOfWork.InstructorProfiles.GetAllAsync(cancellationToken);

        var query = profiles.AsEnumerable();
        if (request.ApprovedOnly.HasValue && request.ApprovedOnly.Value)
        {
            query = query.Where(p => p.IsApproved);
        }

        return _mapper.Map<IReadOnlyList<InstructorProfileDto>>(query.ToList());
    }

    public async Task<InstructorProfileDto> Handle(GetInstructorProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _unitOfWork.InstructorProfiles.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("InstructorProfile", request.Id);

        return _mapper.Map<InstructorProfileDto>(profile);
    }

    public async Task<InstructorProfileDto> Handle(GetMyInstructorProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required.");

        var profiles = await _unitOfWork.InstructorProfiles.GetAsync(p => p.UserId == userId, cancellationToken);
        var profile = profiles.FirstOrDefault()
            ?? throw new NotFoundException("InstructorProfile", userId);

        return _mapper.Map<InstructorProfileDto>(profile);
    }

    public async Task<InstructorWalletDto> Handle(GetInstructorWalletQuery request, CancellationToken cancellationToken)
    {
        Guid instructorId;
        if (request.InstructorId.HasValue)
        {
            instructorId = request.InstructorId.Value;
        }
        else
        {
            var userId = _currentUser.UserId
                ?? throw new DomainException("Authentication required.");
            var profiles = await _unitOfWork.InstructorProfiles.GetAsync(p => p.UserId == userId, cancellationToken);
            var profile = profiles.FirstOrDefault()
                ?? throw new NotFoundException("InstructorProfile", userId);
            instructorId = profile.Id;
        }

        var wallets = await _unitOfWork.Wallets.GetAsync(w => w.InstructorId == instructorId, cancellationToken);
        var wallet = wallets.FirstOrDefault()
            ?? throw new NotFoundException("InstructorWallet", instructorId);

        var txRepo = _unitOfWork.Repository<WalletTransaction>();
        var transactions = await txRepo.GetAsync(t => t.WalletId == wallet.Id, cancellationToken);

        var dto = _mapper.Map<InstructorWalletDto>(wallet);
        dto.RecentTransactions = _mapper.Map<List<WalletTransactionDto>>(transactions.OrderByDescending(t => t.CreatedAtUtc).Take(20).ToList());

        return dto;
    }

    public async Task<PayoutRequestDto> Handle(CreatePayoutRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required.");

        var profiles = await _unitOfWork.InstructorProfiles.GetAsync(p => p.UserId == userId, cancellationToken);
        var profile = profiles.FirstOrDefault()
            ?? throw new BusinessRuleViolationException("NotInstructor", "User does not have an instructor profile.");

        var wallets = await _unitOfWork.Wallets.GetAsync(w => w.InstructorId == profile.Id, cancellationToken);
        var wallet = wallets.FirstOrDefault()
            ?? throw new NotFoundException("InstructorWallet", profile.Id);

        if (wallet.Balance < request.Dto.Amount)
            throw new BusinessRuleViolationException("InsufficientBalance", $"Requested amount {request.Dto.Amount:C} exceeds current wallet balance {wallet.Balance:C}.");

        var payout = new PayoutRequest
        {
            InstructorId = profile.Id,
            Amount = request.Dto.Amount,
            Status = PayoutStatus.Pending,
            PayoutMethod = request.Dto.PayoutMethod,
            Notes = request.Dto.Notes?.Trim()
        };

        // Move amount from available balance to pending balance
        wallet.Balance -= request.Dto.Amount;
        wallet.PendingBalance += request.Dto.Amount;

        _unitOfWork.Wallets.Update(wallet);
        await _unitOfWork.PayoutRequests.AddAsync(payout, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PayoutRequestDto>(payout);
    }

    public async Task<PayoutRequestDto> Handle(ProcessPayoutRequestCommand request, CancellationToken cancellationToken)
    {
        var payout = await _unitOfWork.PayoutRequests.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("PayoutRequest", request.Id);

        if (payout.Status != PayoutStatus.Pending && payout.Status != PayoutStatus.Approved)
            throw new BusinessRuleViolationException("AlreadyProcessed", "This payout request has already been processed.");

        var wallets = await _unitOfWork.Wallets.GetAsync(w => w.InstructorId == payout.InstructorId, cancellationToken);
        var wallet = wallets.FirstOrDefault()
            ?? throw new NotFoundException("InstructorWallet", payout.InstructorId);

        payout.Status = request.Dto.Status;
        payout.Notes = string.IsNullOrWhiteSpace(request.Dto.Notes) ? payout.Notes : request.Dto.Notes;
        payout.ProcessedAtUtc = DateTime.UtcNow;

        if (request.Dto.Status == PayoutStatus.Processed)
        {
            // Deduct from pending balance and record debit transaction
            wallet.PendingBalance -= payout.Amount;

            var tx = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = payout.Amount,
                Type = WalletTransactionType.Debit,
                Description = $"Payout processed via {payout.PayoutMethod}",
                ReferenceId = payout.Id.ToString()
            };
            var txRepo = _unitOfWork.Repository<WalletTransaction>();
            await txRepo.AddAsync(tx, cancellationToken);
        }
        else if (request.Dto.Status == PayoutStatus.Rejected)
        {
            // Refund back to available balance
            wallet.PendingBalance -= payout.Amount;
            wallet.Balance += payout.Amount;
        }

        _unitOfWork.Wallets.Update(wallet);
        _unitOfWork.PayoutRequests.Update(payout);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PayoutRequestDto>(payout);
    }

    public async Task<IReadOnlyList<PayoutRequestDto>> Handle(GetPayoutRequestsQuery request, CancellationToken cancellationToken)
    {
        var payouts = await _unitOfWork.PayoutRequests.GetAllAsync(cancellationToken);

        var query = payouts.AsEnumerable();
        if (request.InstructorId.HasValue)
        {
            query = query.Where(p => p.InstructorId == request.InstructorId.Value);
        }

        return _mapper.Map<IReadOnlyList<PayoutRequestDto>>(query.OrderByDescending(p => p.CreatedAtUtc).ToList());
    }
}
