﻿using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using PixelHotel.Core.Domain;
using PixelHotel.Core.Events;
using PixelHotel.Core.Events.Abstractions;
using System.Reflection;

namespace PixelHotel.Infra.Configurations;

public static class DbContextConfiguration
{
    public static ModelBuilder ConfigureDefault(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var propertyZuadas = modelBuilder
            .Model
            .GetEntityTypes()
            .SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string)));

        foreach (var property in propertyZuadas)
            property.SetColumnType("VARCHAR(255)");

        modelBuilder.Ignore<ValidationResult>();
        modelBuilder.Ignore<Event>();

        return modelBuilder;
    }

    public static async Task<bool> Commit(this DbContext context, IPublisherEvent publisher)
    {
        await publisher.PublishDomainEvents(context).ConfigureAwait(false);
        return await context.SaveChangesAsync() > 0;
    }
}