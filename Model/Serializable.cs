using System;
using System.Collections.Generic;
using System.Text;

namespace Dombo.CommonModel
{
    public interface ISerializableResult
    {
        string SerializeObject(object jsonObject);
    }
}
