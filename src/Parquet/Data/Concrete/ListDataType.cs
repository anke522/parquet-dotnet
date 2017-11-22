﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Parquet.Data;

namespace Parquet.Data.Concrete
{
   class ListDataType : IDataTypeHandler
   {
      public ListDataType()
      {
      }

      public int? BitWidth => null;

      public DataType DataType => DataType.Unspecified;

      public SchemaType SchemaType => SchemaType.Structure;

      public Type ClrType => typeof(IEnumerable<>);

      public IList CreateEmptyList(bool isNullable, bool isArray, int capacity)
      {
         throw new NotImplementedException();
      }

      public Field CreateSchemaElement(IList<Thrift.SchemaElement> schema, ref int index, out int ownedChildCount)
      {
         var list = new ListField(schema[index].Name);

         //skip this element and child container
         index += 2;

         //parent parser should add more elements inside this structure

         throw new NotImplementedException();
         ownedChildCount = 0;
         return list;
      }

      public void CreateThrift(Field se, Thrift.SchemaElement parent, IList<Thrift.SchemaElement> container)
      {
         throw new NotImplementedException();
      }

      public bool IsMatch(Thrift.SchemaElement tse, ParquetOptions formatOptions)
      {
         return tse.__isset.converted_type && tse.Converted_type == Thrift.ConvertedType.LIST;
      }

      public IList Read(Thrift.SchemaElement tse, BinaryReader reader, ParquetOptions formatOptions)
      {
         throw new NotImplementedException();
      }

      public void Write(Thrift.SchemaElement tse, BinaryWriter writer, IList values)
      {
         throw new NotImplementedException();
      }
   }
}
