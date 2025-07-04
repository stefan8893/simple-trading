﻿using SimpleTrading.Domain.Trading.UseCases.Profiles.GetProfiles;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public class ProfileDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required bool IsActive { get; init; }

    public static ProfileDto From(GetProfilesResponseModel model)
    {
        return new ProfileDto
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            IsActive = model.IsActive
        };
    }
}