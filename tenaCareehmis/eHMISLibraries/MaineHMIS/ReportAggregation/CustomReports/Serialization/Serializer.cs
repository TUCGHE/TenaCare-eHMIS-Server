/*
 * 
 * Copyright © 2006-2017 TenaCareeHMIS  software, by The Administrators of the Tulane Educational Fund, 
 * dba Tulane University, Center for Global Health Equity is distributed under the GNU General Public License(GPL).
 * All rights reserved.

 * This file is part of TenaCareeHMIS
 * TenaCareeHMIS is free software: 
 * 
 * you can redistribute it and/or modify it under the terms of the 
 * GNU General Public License as published by the Free Software Foundation, 
 * version 3 of the License, or any later version.
 * TenaCareeHMIS is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
 * FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License along with TenaCareeHMIS.  
 * If not, see http://www.gnu.org/licenses/.    
 * 
 * 
*/

using System.IO;
using System.Text;
//using Interfaces;

namespace eHMIS.HMIS.ReportAggregation.CustomReports.Serialization
{

    public class ProtoBufferSerializer : ISerializer
    {
        public byte[] Serialize<K>(K model)
        {
            var stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize<K>(stream, model);
            return stream.ToArray();
        }
        public K Deserialize<K>(byte[] data)
        {
            MemoryStream memStream = new MemoryStream(data);
            return ProtoBuf.Serializer.Deserialize<K>(memStream);
        }
    }
}
