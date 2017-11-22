﻿using System;
using System.Collections.Generic;
using System.Linq;
using Parquet.Data.Concrete;

namespace Parquet.Data
{
   static class DataTypeFactory
   {
      private static readonly List<IDataTypeHandler> _allDataTypes = new List<IDataTypeHandler>
      {
         // special types
         new DateTimeOffsetDataType(),
         new DateTimeDataType(),
         new IntervalDataType(),
         new DecimalDataType(),

         // low priority types
         new BooleanDataType(),
         new ByteDataType(),
         new SignedByteDataType(),
         new Int16DataType(),
         new UnsignedInt16DataType(),
         new Int32DataType(),
         new Int64DataType(),
         new Int96DataType(),
         new FloatDataType(),
         new DoubleDataType(),
         new StringDataType(),
         new ByteArrayDataType(),

         // composite types
         new ListDataType(),
         new MapDataType(),
         new StructureDataType()
      };

      //todo: all the matches can be much faster, cache them.

      public static IDataTypeHandler Match(Thrift.SchemaElement tse, ParquetOptions formatOptions)
      {
         return _allDataTypes.FirstOrDefault(dt => dt.IsMatch(tse, formatOptions));
      }

      public static IDataTypeHandler Match(DataType dataType)
      {
         return _allDataTypes.FirstOrDefault(dt => dt.DataType == dataType);
      }

      public static IDataTypeHandler Match(Field field)
      {
         switch(field.SchemaType)
         {
            case SchemaType.Map:
               return new MapDataType();
            case SchemaType.List:
               return new ListDataType();
            case SchemaType.PrimitiveType:
               return Match(((DataField)field).DataType);
            default:
               throw new NotImplementedException();
         }
      }

      public static IDataTypeHandler Match(Type clrType)
      {
         return _allDataTypes.FirstOrDefault(dt => dt.ClrType == clrType);
      }

      public static void ThrowClrTypeNotSupported(Type clrType)
      {
         string message = string.Format("CLR type '{0}' is not supported, please specify one of '{1}' or use an alternative constructor",
            clrType,
            string.Join(", ", _allDataTypes.Select(dt => dt.ClrType))
            );

         throw new NotSupportedException(message);
      }
   }
}
