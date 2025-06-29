﻿using System.Data;
using System.Text.Json;
using Dapper;
using DirectoryProject.DirectoryService.Application.DTOs;

namespace DirectoryProject.DirectoryService.Infrastructure.DatabaseRead;

public static class DapperConfigurationHelper
{
    public static void Configure()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        SqlMapper.AddTypeHandler(new JsonTypeHandler<AddressDTO>());
    }

    public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
    {
        public override T Parse(object value)
        {
            string json = (string)value;
            return JsonSerializer.Deserialize<T>(json);
        }

        public override void SetValue(IDbDataParameter parameter, T value)
        {
            parameter.Value = JsonSerializer.Serialize(value);
        }
    }
}
