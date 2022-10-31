﻿using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace HT.Extensions.SqlClient
{
    public static partial class SqlParametersExtensions
    {
        public static SqlCommand AddVarBinary(this SqlCommand command, string parameterName, byte[] parameterValue)
        {
            command.Parameters.AddVarBinary(parameterName, parameterValue);
            return command;
        }
        public static SqlCommand AddBinary(this SqlCommand command, string parameterName, byte[] parameterValue)
        {
            command.Parameters.AddBinary(parameterName, parameterValue);
            return command;
        }
        public static SqlCommand AddVarBinaryJson(this SqlCommand command, string parameterName,
           object objectToSerialize)
        {
            command.Parameters.AddVarBinaryJson(parameterName, objectToSerialize);
            return command;
        }
        public static SqlCommand AddNVarchar(this SqlCommand command, string parameterName, string parameterValue)
        {
            command.Parameters.AddNVarchar(parameterName, parameterValue);
            return command;
        }
        public static SqlCommand AddVarchar(this SqlCommand command, string parameterName, string parameterValue)
        {
            command.Parameters.AddVarchar(parameterName, parameterValue);
            return command;
        }
        public static SqlCommand AddNullable(this SqlCommand command, string parameterName, object parameterValue)
        {
            command.Parameters.AddNullable(parameterName, parameterValue);
            return command;
        }
        public static SqlCommand AddNVarCharJson(this SqlCommand command, string parameterName, object objectToSerialize)
        {
            command.Parameters.AddNVarCharJson(parameterName, objectToSerialize);
            return command;
        }
        public static SqlCommand AddBit(this SqlCommand command, string parameterName, bool? parameterValue)
        {
            command.Parameters.AddBit(parameterName, parameterValue);
            return command;
        }
        public static SqlCommand AddDateTimeOffset(this SqlCommand command, string parameterName, DateTimeOffset? parameterValue)
        {
            command.Parameters.AddDateTimeOffset(parameterName, parameterValue);
            return command;
        }

        public static SqlCommand AddDateTime(this SqlCommand command, string parameterName, DateTimeOffset parameterValue)
        {
            command.Parameters.AddDateTime(parameterName, parameterValue);
            return command;
        }
        public static SqlCommand AddDateTime(this SqlCommand command, string parameterName, DateTime parameterValue)
        {
            command.Parameters.AddDateTime(parameterName, parameterValue);
            return command;
        }
        public static SqlCommand AddInteger(this SqlCommand command, string parameterName, int parameterValue)
        {
            command.Parameters.AddInteger(parameterName, parameterValue);
            return command;
        }
        public static SqlCommand AddInteger(this SqlCommand command, string parameterName, int? parameterValue)
        {
            command.Parameters.AddInteger(parameterName, parameterValue);
            return command;
        }
        public static SqlCommand AddDouble(this SqlCommand command, string parameterName, double parameterValue)
        {
            command.Parameters.AddDouble(parameterName, parameterValue);
            return command;
        }
        public static SqlCommand AddBigInt(this SqlCommand command, string parameterName, long parameterValue, ParameterDirection parameterDirection = ParameterDirection.Input)
        {
            command.Parameters.AddBigInt(parameterName, parameterValue, parameterDirection);
            return command;
        }

        public static SqlCommand AddBigInt(this SqlCommand command, string parameterName, long? parameterValue,
            ParameterDirection parameterDirection = ParameterDirection.Input)
        {
            command.Parameters.AddBigInt(parameterName, parameterValue, parameterDirection);
            return command;
        }


    }
}
