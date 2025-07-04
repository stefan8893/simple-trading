﻿using SimpleTrading.Domain.Infrastructure.Extensions;

namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public static partial class TestData
{
    public record Profile : ITestData<Domain.Trading.Profile, Profile>
    {
        private static short _profileNumber = 1;
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; init; } = $"Test Profile - {_profileNumber++:00}";
        public string? Description { get; init; } = null;
        public bool IsActive { get; init; }
        public DateTime Created { get; init; } = DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();

        public static Profile Default => new();

        public Domain.Trading.Profile Build()
        {
            return new Domain.Trading.Profile
            {
                Id = Id,
                Name = Name,
                Description = Description,
                IsActive = IsActive,
                Created = Created
            };
        }
    }
}