﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
    public enum Auto {
        yes,no
    }

    public enum ConfigItemName
    {
        //connectionStrings
        appSettings,
        connectionStrings,

        //appSettings
        programVersion,
        coryRight,
        dbType,
    }
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DbType
    {
        mysql,
        sqlite
    }

    public enum DataMoveType {
        Schema,
        Data,
        SchemaAndData,
    }
}
