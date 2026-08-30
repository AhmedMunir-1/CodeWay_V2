namespace CodeWay.Application.Features.Payments.Queries;

using AutoMapper;
using CodeWay.Application.Contracts;
using CodeWay.Application.Features.Payments.DTOs;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record GetPaymentsQuery(Guid? UserId = null, PaymentStatus? Status = null) : IRequest<IReadOnlyList<PaymentDto>>;

public sealed record GetPaymentByIdQuery(Guid Id) : IRequest<PaymentDto>;

public sealed class PaymentQueryHandler :
    IRequestHandler<GetPaymentsQuery, IReadOnlyList<PaymentDto>>,
    IRequestHandler<GetPaymentByIdQuery, PaymentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public PaymentQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PaymentDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var payments = await _unitOfWork.Payments.GetAllAsync(cancellationToken);

        var query = payments.AsEnumerable();

        if (request.UserId.HasValue)
        {
            query = query.Where(p => p.UserId == request.UserId.Value);
        }
        else if (_currentUser.UserId.HasValue)
        {
            query = query.Where(p => p.UserId == _currentUser.UserId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(p => p.Status == request.Status.Value);
        }

        return _mapper.Map<IReadOnlyList<PaymentDto>>(query.OrderByDescending(p => p.CreatedAtUtc).ToList());
    }

    public async Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Payment", request.Id);

        return _mapper.Map<PaymentDto>(payment);
    }
}
