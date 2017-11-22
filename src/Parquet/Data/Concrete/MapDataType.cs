﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Parquet.Data;

namespace Parquet.Data.Concrete
{
   class MapDataType : IDataTypeHandler
   {
      public MapDataType()
      {
      }

      public int? BitWidth => null;

      public DataType DataType => DataType.Unspecified;

      public SchemaType SchemaType => SchemaType.Structure;

      public Type ClrType => typeof(IDictionary<,>);

      public IList CreateEmptyList(bool isNullable, bool isArray, int capacity)
      {
         throw new NotImplementedException();
      }

      public Field CreateSchemaElement(IList<Thrift.SchemaElement> schema, ref int index, out int ownedChildCount)
      {
         Thrift.SchemaElement tseRoot = schema[index];

         //next element is a container
         Thrift.SchemaElement tseContainer = schema[++index];

         if(tseContainer.Num_children != 2)
         {
            throw new IndexOutOfRangeException($"dictionary container must have exactly 2 children but {tseContainer.Num_children} found");
         }

         //followed by a key and a value, but we declared them as owned

         var map = new MapField(tseRoot.Name);
         map.Path = tseRoot.Name + Schema.PathSeparator + tseContainer.Name;
         index += 1;
         ownedChildCount = 2;
         return map;
      }

      public void CreateThrift(Field se, Thrift.SchemaElement parent, IList<Thrift.SchemaElement> container)
      {
         parent.Num_children += 1;

         //add the root container where map begins
         var root = new Thrift.SchemaElement(se.Name)
         {
            Converted_type = Thrift.ConvertedType.MAP,
            Num_children = 1,
            Repetition_type = Thrift.FieldRepetitionType.OPTIONAL
         };
         container.Add(root);

         //key-value is a container for column of keys and column of values
         var keyValue = new Thrift.SchemaElement("key_value")
         {
            Num_children = 0, //is assigned by children
            Repetition_type = Thrift.FieldRepetitionType.REPEATED
         };
         container.Add(keyValue);

         //now add the key and value separately
         MapField dse = se as MapField;
         IDataTypeHandler keyHandler = DataTypeFactory.Match(dse.Key.DataType);
         IDataTypeHandler valueHandler = DataTypeFactory.Match(dse.Value.DataType);

         keyHandler.CreateThrift(dse.Key, keyValue, container);
         Thrift.SchemaElement tseKey = container[container.Count - 1];
         valueHandler.CreateThrift(dse.Value, keyValue, container);
         Thrift.SchemaElement tseValue = container[container.Count - 1];

         //fixups for weirdness in RLs
         if (tseKey.Repetition_type == Thrift.FieldRepetitionType.REPEATED)
            tseKey.Repetition_type = Thrift.FieldRepetitionType.OPTIONAL;
         if (tseValue.Repetition_type == Thrift.FieldRepetitionType.REPEATED)
            tseValue.Repetition_type = Thrift.FieldRepetitionType.OPTIONAL;
      }

      public bool IsMatch(Thrift.SchemaElement tse, ParquetOptions formatOptions)
      {
         return
            tse.__isset.converted_type &&
            (tse.Converted_type == Thrift.ConvertedType.MAP || tse.Converted_type == Thrift.ConvertedType.MAP_KEY_VALUE);
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
