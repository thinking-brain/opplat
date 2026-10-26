using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Infrastructure.Persistance.Configurations.Sales;

public sealed class PosRegisterConfiguration : IEntityTypeConfiguration<PosRegister>
{
    public void Configure(EntityTypeBuilder<PosRegister> builder)
    {
        builder.Property(register => register.Code).IsRequired().HasMaxLength(20);
        builder.Property(register => register.Name).IsRequired().HasMaxLength(100);
        // The series ends up in AEAT's NumSerieFactura (max 60 chars with the sequence).
        builder.Property(register => register.InvoiceSeries).IsRequired().HasMaxLength(20);
        builder.HasIndex(register => register.Code).IsUnique();
        builder.HasIndex(register => register.InvoiceSeries).IsUnique();
    }
}

public sealed class PosShiftConfiguration : IEntityTypeConfiguration<PosShift>
{
    public void Configure(EntityTypeBuilder<PosShift> builder)
    {
        builder.Property(shift => shift.OpenedBy).IsRequired();
        builder.HasOne(shift => shift.Register).WithMany().HasForeignKey(shift => shift.RegisterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(shift => shift.CashMovements).WithOne(movement => movement.Shift).HasForeignKey(movement => movement.ShiftId);
        // One open shift per register, enforced by the database.
        builder.HasIndex(shift => shift.RegisterId).IsUnique().HasFilter("\"Status\" = 0");
    }
}

public sealed class PosCashMovementConfiguration : IEntityTypeConfiguration<PosCashMovement>
{
    public void Configure(EntityTypeBuilder<PosCashMovement> builder)
    {
        builder.Property(movement => movement.Reason).IsRequired();
        builder.Property(movement => movement.User).IsRequired();
    }
}

public sealed class PaymentTerminalConfiguration : IEntityTypeConfiguration<PaymentTerminal>
{
    public void Configure(EntityTypeBuilder<PaymentTerminal> builder)
    {
        builder.Property(terminal => terminal.Provider).IsRequired().HasMaxLength(50);
        builder.Property(terminal => terminal.ProviderTerminalId).IsRequired().HasMaxLength(100);
        builder.Property(terminal => terminal.DisplayName).IsRequired().HasMaxLength(100);
        builder.HasOne(terminal => terminal.Register).WithMany().HasForeignKey(terminal => terminal.RegisterId).OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(terminal => new { terminal.Provider, terminal.ProviderTerminalId }).IsUnique();
    }
}

public sealed class SalePaymentConfiguration : IEntityTypeConfiguration<SalePayment>
{
    public void Configure(EntityTypeBuilder<SalePayment> builder)
    {
        builder.HasOne(payment => payment.Sale).WithMany(sale => sale.Payments).HasForeignKey(payment => payment.SaleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(payment => payment.Attempts).WithOne(attempt => attempt.SalePayment).HasForeignKey(attempt => attempt.SalePaymentId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
{
    public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
    {
        builder.Property(attempt => attempt.Provider).IsRequired().HasMaxLength(50);
        builder.Property(attempt => attempt.IdempotencyKey).IsRequired().HasMaxLength(100);
        builder.Property(attempt => attempt.CardLast4).HasMaxLength(4);
        builder.HasOne(attempt => attempt.Terminal).WithMany().HasForeignKey(attempt => attempt.TerminalId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(attempt => attempt.IdempotencyKey).IsUnique();
        builder.HasIndex(attempt => new { attempt.Provider, attempt.ProviderReference });
    }
}
