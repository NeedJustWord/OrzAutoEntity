# OrzAutoEntity

#### 工具名称：

Visual Studio扩展[实体类生成工具](https://github.com/NeedJustWord/OrzAutoEntity)



#### 作用：

读取数据库表和视图结构信息，根据模板生成对应的实体类



#### 支持的Visual Studio：

1. Visual Studio 2019
2. Visual Studio 2022



#### 支持的数据库类型：

| 数据库类型 | 数据库连接字符串                                             | 适用最低版本 |
| ---------- | ------------------------------------------------------------ | ------------ |
| Oracle     | Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=MyHost)(PORT=MyPort)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=MyOracleSID)));User Id=myUsername;Password=myPassword; | v1.0         |
| Dm         | Server=myServerAddress;Port=myPort;Uid=myUsername;Pwd=myPassword; | v1.0         |
| Gbase      | DSN=myDsn;                                                   | v1.0         |
| Sybase     | Server=myServerAddress;Port=myPort;Database=myDatabase;Uid=myUsername;Pwd=myPassword; | v1.0         |
| MySql      | Server=myServerAddress;Database=myDatabase;Uid=myUsername;Pwd=myPassword; | v1.0         |
| Sqlite     | Data Source=c:\mydb.db;Version=3;                            | v1.0         |
| SqlServer  | Data Source=myServerAddress;Initial Catalog=myDatabase;User ID=myUsername;Password=myPassword;TrustServerCertificate=True; | v1.1         |



#### 可用模板变量：

##### 表变量$Table：

表变量为`$Table`，用法：`$Table.Name`

| 属性名         | 含义                                                    | 适用最低版本 |
| -------------- | ------------------------------------------------------- | ------------ |
| Name           | 表名/视图名                                             | v1.0         |
| CamelName      | 大驼峰格式的表名/视图名。v2.0版本后是大驼峰格式的实体名 | v1.0         |
| LowerName      | 小写格式的表名/视图名。v2.0版本后是小写格式的实体名     | v1.0         |
| IsView         | 是否是视图                                              | v1.0         |
| Comment        | 注释                                                    | v1.0         |
| Columns        | 字段信息集合                                            | v1.0         |
| EntityName     | 实体名，默认是Name。可通过配置修改，具体看配置文档说明  | v2.0         |
| LowerCamelName | 小驼峰格式的实体名                                      | v2.0         |
| FilePath       | 实体类生成路径，可通过配置修改，具体看配置文档说明      | v2.0         |



##### 字段变量$c：

在模板里使用`#foreach($c in $Table.Columns)`遍历表字段信息集合时，字段变量就是`$c`，用法：`$c.Name`

| 属性名         | 含义                                                   | 适用最低版本 |
| -------------- | ------------------------------------------------------ | ------------ |
| Name           | 列名                                                   | v1.0         |
| CamelName      | 大驼峰格式的列名。v2.0版本后是大驼峰格式的字段名       | v1.0         |
| LowerName      | 小写格式的列名。v2.0版本后是大驼峰格式的字段名         | v1.0         |
| Comment        | 注释                                                   | v1.0         |
| DbType         | 数据库类型                                             | v1.0         |
| ClrType        | CLR类型                                                | v1.0         |
| TypeInTable    | 表字段类型，根据AllowNull确定是否允许空                | v1.0         |
| TypeInView     | 视图字段类型，可空字段类型                             | v1.0         |
| Length         | 长度                                                   | v1.0         |
| Precision      | 数字精度                                               | v1.0         |
| Scale          | 数字标度                                               | v1.0         |
| AllowNull      | 是否允许空                                             | v1.0         |
| IsKey          | 是否主键                                               | v1.0         |
| Identity       | 是否自增                                               | v1.0         |
| FieldName      | 字段名，默认是Name。可通过配置修改，具体看配置文档说明 | v2.0         |
| LowerCamelName | 小驼峰格式的字段名                                     | v2.0         |



#### 配置文档说明：

##### Templates配置：

Templates是模板配置集合，其下每个Template为一个模板配置

| Template属性名 | 含义                                           | 适用最低版本 |
| -------------- | ---------------------------------------------- | ------------ |
| id             | 模板配置id，集合内不重复                       | v1.0         |
| 文本内容       | 模板内容                                       | v1.0         |
| filePath       | 配置实体类路径和名称，支持表变量$Table里的属性 | v2.0         |



##### Filters配置：

Filters是匹配配置集合，可对数据库和实体类进行匹配并执行操作，其下每个Filter为一个匹配配置

| Filter属性名 | 含义                           | 适用最低版本 |
| ------------ | ------------------------------ | ------------ |
| id           | 匹配配置id，集合内不重复       | v1.0         |
| type         | 匹配类型，值为`FilterType`枚举 | v2.0         |

| FilterType枚举 | 含义                         | 适用最低版本 |
| -------------- | ---------------------------- | ------------ |
| table          | 默认值，表示此匹配是针对表的 | v2.0         |
| model          | 表示此匹配是针对实体类的     | v2.0         |

Filter下是Item集合，每个Item是具体的匹配配置，在属性里定义该匹配执行的操作

| Filter.Item属性名 | 含义                                                         | 适用最低版本 |
| ----------------- | ------------------------------------------------------------ | ------------ |
| 文本内容          | 匹配表名或实体类文件名，形如sql里like语法的条件规则，多个条件时用竖线`\|`分隔 | v1.0         |
| operate           | 匹配执行操作，值为`FilterOperate`枚举                        | v2.0         |
| value             | 设置值，operate属性值为`setPath`或`setEntityName`时有效      | v2.0         |

| FilterOperate枚举 | 含义                                                         | 适用最低版本 |
| ----------------- | ------------------------------------------------------------ | ------------ |
| skip              | 默认值，忽略操作，匹配的表或实体类会被忽略。v1.0版本相当于是此操作 | v2.0         |
| include           | 包含操作，匹配的表或实体类会被包含                           | v2.0         |
| trim              | 移除前导匹配项或尾部匹配项操作，匹配类型为`table`时有效，匹配的表名会移除匹配项后设置成实体名，形如`$Table.EntityName=$Table.Name.TrimStart(value)`、`$Table.EntityName=$Table.Name.TrimEnd(value)` | v2.0         |
| setPath           | 设置路径操作，匹配类型为`table`时有效，设置匹配的表的实体类生成路径，形如`$Table.FilePath=value` | v2.0         |
| setEntityName     | 设置实体名操作，匹配类型为`table`时有效，设置匹配的表的实体名，形如`$Table.EntityName=value` | v2.0         |



##### Columns配置：

Columns是`2.0`版本新增的字段配置集合，可对表或视图的字段名进行定制化配置，其下每个Column为一个针对表或视图的字段配置

| Column属性名 | 含义                                                         | 适用最低版本 |
| ------------ | ------------------------------------------------------------ | ------------ |
| id           | 字段配置id，集合内不重复                                     | v2.0         |
| tableName    | 表名或视图名，非空时针对指定表或视图，为空时针对所有表或视图 | v2.0         |

Column下是Item集合，每个Item是具体的字段配置

| Column.Item属性名 | 含义                                                       | 适用最低版本 |
| ----------------- | ---------------------------------------------------------- | ------------ |
| columnName        | 字段名，要定制化配置的字段                                 | v2.0         |
| skip              | 生成实体类时是否跳过                                       | v2.0         |
| clrType           | 指定该字段生成的实体类类型，形如`$c.ClrType=clrType`       | v2.0         |
| fieldName         | 指定该字段生成的实体类字段名，形如`$c.FieldName=fieldName` | v2.0         |



##### TypeMapping配置：

TypeMapping是数据库类型和CLR类型转换配置集合，其下每个Mapping为一个针对指定数据库的转换配置

| Mapping属性名 | 含义                                     | 适用最低版本 |
| ------------- | ---------------------------------------- | ------------ |
| name          | 数据库类型名，配置值参考支持的数据库类型 | v1.0         |

Mapping下是Item集合，每个Item是具体的数据库类型和CLR类型转换配置

| Mapping.Item属性名 | 含义                                                         | 适用最低版本 |
| ------------------ | ------------------------------------------------------------ | ------------ |
| sqlType            | 和`isNumber`一起使用时是字段数据库类型，对应`$c.DbType`；和`clrType`一起使用时是转换前的数据库类型，对应`$c.DbType`或`$c.DbType($c.Precision,$c.Scale)`，多个时用竖线`\|`分隔 | v1.0         |
| isNumber           | 默认转换前的数据库类型为`$c.DbType`，此值为`Y`时表示此数据库类型是数字类型，转换前的数据库类型为`$c.DbType($c.Precision,$c.Scale)` | v1.0         |
| clrType            | 转换后的CLR类型                                              | v1.0         |



##### DataSource配置：

DataSource是数据库配置集合，其下每个Database为一个针对指定数据库进行实体类生成的配置

| Database属性名 | 含义                                                         | 适用最低版本 |
| -------------- | ------------------------------------------------------------ | ------------ |
| name           | 数据库名称，集合内不重复                                     | v1.0         |
| type           | 数据库类型名，配置值参考支持的数据库类型                     | v1.0         |
| templateId     | 模板配置id集合，`Templates`配置里的id，多个用英文逗号分隔    | v1.0         |
| filterId       | 匹配配置id集合，`Filters`配置里的id，多个用英文逗号分隔      | v1.0         |
| directory      | 实体类生成目录，v2.0版本后支持多层目录                       | v1.0         |
| connString     | 数据库连接字符串                                             | v1.0         |
| columnId       | 字段配置id集合，`Columns`配置里的id，多个用英文逗号分隔      | v2.0         |
| isSelected     | 界面加载时是否选中，默认选中第一个配置true的，否则选中第一个`Database`配置 | v2.0         |



##### 实体类生成目录配置说明：

**v1.0版本**只有**Database.directory**(配置1)一个地方配置实体类生成目录，只支持一层目录。最终生成实体类文件：\配置1\\${Table.Name}.cs

**v2.0版本**增强了**Database.directory**(配置1)，支持多层目录。另外新增两个相关配置，分别是**Filters**下的**Filter.Item.operate=setPath**(配置2)和**Templates**下的**Template.filePath**(配置3)，具体配置方式请查看对应配置说明。最终生成实体类文件：

- 当配置3为空时，为：\配置1\配置2\\$Table.LowerName.cs
- 当配置3不为空时，为：\配置1\配置2\配置3

> 最终生成实体类文件路径都是相对项目文件*.csproj所在目录



#### [v2.0](https://github.com/NeedJustWord/OrzAutoEntity/blob/main/Vsixs/OrzAutoEntity%20v2.0.zip)更新日志：

1. 配置文件增强，满足复杂多样的实体类生成需要
2. 可用模板变量添加新属性
3. 界面加载时默认选中第一个isSelected配置true的Database，否则选中第一个Database配置
4. 代码优化及重构

```xml
<?xml version="1.0" encoding="utf-8" ?>
<AutoEntity>
    <!--模板配置，可以有多个Template节点-->
    <Templates>
        <!--id:模板id；<![CDATA[XXX]]>:XXX为模板内容-->
        <Template id="1" filePath="${Table.CamelName}.cs">
            <![CDATA[
/* ============================
 * 本文件由实体类生成工具生成，请勿手动更改
 * ============================ */
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Oracle
{
#if($Table.Comment!="")
    /// <summary>
    /// $Table.Comment
    /// </summary>
#end
    public class $Table.Name
    {
#foreach($c in $Table.Columns)
#if($c.Comment!="")
        /// <summary>
        /// $c.Comment
        /// </summary>
#end
#if($c.IsKey)
        [Key]
#end
#if($c.Identity)
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
#end
#if(!$Table.IsView&&!$c.AllowNull)
        [Required]
#end
        [Column("$c.Name")]
#if($Table.IsView)
        public $c.TypeInView $c.CamelName{ get; set; }
#else
        public $c.TypeInTable $c.CamelName{ get; set; }
#end
#end
    }
}
            ]]>
        </Template>
    </Templates>

    <!--匹配配置，可以有多个Filter节点-->
    <Filters>
        <!--id:匹配id，type:匹配类型，表-->
        <Filter id="1" type="table">
            <!--匹配表名以X1或X2结尾的，执行过滤操作-->
            <Item>%X1|X2</Item>
            <!--匹配表名以X1或X2开头的，执行过滤操作-->
            <Item operate="skip">X1|X2%</Item>
            <!--匹配表名包含X1或X2的，执行包含操作-->
            <Item operate="include">%X1|X2%</Item>
            <!--匹配表名以X1或X2开头的，设置其实体名为其表名去掉X1或X2前缀-->
            <Item operate="trim">X1|X2%</Item>
            <!--匹配表名以X1或X2结尾的，设置其实体名为其表名去掉X1或X2后缀-->
            <Item operate="trim">%X1|X2</Item>
            <!--匹配表名为X1或X2的，设置其实体类生成路径为path1\path2-->
            <Item operate="setPath" value="path1\path2">X1|X2</Item>
            <!--匹配表名为X1的，设置其实体名为entity-->
            <Item operate="setEntityName" value="entity">X1</Item>
        </Filter>
        <!--id:匹配id，type:匹配类型，实体类-->
        <Filter id="2" type="model">
            <!--匹配实体类文件名以X1或X2结尾的，执行过滤操作-->
            <Item>%X1|X2</Item>
            <!--匹配实体类文件名以X1或X2开头的，执行过滤操作-->
            <Item operate="skip">X1|X2%</Item>
            <!--匹配实体类文件名包含X1或X2的，执行包含操作-->
            <Item operate="include">%X1|X2%</Item>
            <!--匹配实体类文件名为X1或X2，执行包含操作-->
            <Item operate="include">X1|X2</Item>
        </Filter>
    </Filters>

    <!--数据库表字段配置，可以有多个Column节点-->
    <Columns>
        <!--字段配置,其下可以有多个Item节点。id:字段配置id；tableName:表名或视图名，非空时针对指定表或视图，为空时针对所有表或视图-->
        <Column id="1" tableName="">
            <!--columnName:字段名；skip:生成实体类时是否跳过；clrType:字段类型；FieldName:字段名-->
            <Item columnName="id" skip="true" clrType="Guid" fieldName="key"/>
        </Column>
    </Columns>

    <!--数据库类型匹配配置，可以有多个Mapping节点-->
    <TypeMapping>
        <!--name:数据库类型-->
        <Mapping name="Oracle">
            <!--sqlType:数据库里的类型名,多个用|分隔；isNumber:Y表示此类型是数字-->
            <!--数字类型会转换成【类型名(精度/字节数,标度)】的格式参与匹配，是精度还是字节数取决于数据库类型和字段类型-->
            <Item sqlType="NUMBER" isNumber="Y"></Item>
            <!--sqlType:转换前的类型，【类型名】或【类型名(精度/字节数,标度)】,多个用|分隔；clrType:转换后的类型-->
            <!--重复定义以最后的定义进行匹配-->
            <Item sqlType="CHAR|VARCHAR|VARCHAR2" clrType="string"></Item>
            <Item sqlType="NUMBER(1,0)|NUMBER(2,0)" clrType="byte"></Item>
            <Item sqlType="NUMBER(3,0)|NUMBER(4,0)" clrType="short"></Item>
        </Mapping>
    </TypeMapping>

    <!--数据库配置，可以有多个Database节点-->
    <DataSource>
        <!--name:数据库名字；type:数据库类型；templateId:模板id集合；filterId:匹配id集合；columnId:字段配置id集合；directory:实体类生成目录；isSelected:界面加载时是否选中；connString:数据库连接字符串-->
        <Database name="name" type="Oracle" templateId="1" filterId="1" columnId="1" directory="Oracle" isSelected="true" connString="*"/>
    </DataSource>
</AutoEntity>
```



#### [v1.1](https://github.com/NeedJustWord/OrzAutoEntity/blob/main/Vsixs/OrzAutoEntity%20v1.1.zip)更新日志：

1. 支持Sql Server数据库



#### [v1.0](https://github.com/NeedJustWord/OrzAutoEntity/blob/main/Vsixs/OrzAutoEntity%20v1.0.zip)使用方法：

1. 安装扩展文件
2. 将下面的配置文件保存成 **__entity.xml** 文件名(开头两个下划线)，放到项目文件 ***.csproj** 所在目录
3. 按实际需要修改配置文件
4. Visual Studio的**解决方案资源管理器**中右键点击项目文件，点击**实体类生成工具**菜单
5. 在弹出的窗口中**选择数据源**，**勾选需要生成的项**，点击**添加/刷新选中项**按钮生成实体类

```xml
<?xml version="1.0" encoding="utf-8" ?>
<AutoEntity>
    <!--模板配置，可以有多个Template节点-->
    <Templates>
        <!--id:模板id；<![CDATA[XXX]]>:XXX为模板内容-->
        <Template id="1">
            <![CDATA[
/* ============================
 * 本文件由实体类生成工具生成，请勿手动更改
 * ============================ */
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Oracle
{
#if($Table.Comment!="")
    /// <summary>
    /// $Table.Comment
    /// </summary>
#end
    public class $Table.Name
    {
#foreach($c in $Table.Columns)
#if($c.Comment!="")
        /// <summary>
        /// $c.Comment
        /// </summary>
#end
#if($c.IsKey)
        [Key]
#end
#if($c.Identity)
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
#end
#if(!$Table.IsView&&!$c.AllowNull)
        [Required]
#end
        [Column("$c.Name")]
#if($Table.IsView)
        public $c.TypeInView $c.CamelName{ get; set; }
#else
        public $c.TypeInTable $c.CamelName{ get; set; }
#end
#end
    }
}
            ]]>
        </Template>
    </Templates>

    <!--数据库表过滤配置，符合条件的表不会生成实体类，可以有多个Filter节点-->
    <Filters>
        <!--id:过滤器id-->
        <Filter id="1">
            <!--过滤表名以X1或X2结尾的-->
            <Item>%X1|X2</Item>
            <!--过滤表名以X1或X2开头的-->
            <Item>X1|X2%</Item>
            <!--过滤表名包含X1或X2的-->
            <Item>%X1|X2%</Item>
            <!--过滤表名为X1或X2的-->
            <Item>X1|X2</Item>
        </Filter>
    </Filters>

    <!--数据库类型匹配配置，可以有多个Mapping节点-->
    <TypeMapping>
        <!--name:数据库类型-->
        <Mapping name="Oracle">
            <!--sqlType:数据库里的类型名,多个用|分隔；isNumber:Y表示此类型是数字-->
            <!--数字类型会转换成【类型名(精度/字节数,标度)】的格式参与匹配，是精度还是字节数取决于数据库类型和字段类型-->
            <Item sqlType="NUMBER" isNumber="Y"></Item>
            <!--sqlType:转换前的类型，【类型名】或【类型名(精度/字节数,标度)】,多个用|分隔；clrType:转换后的类型-->
            <!--重复定义以最后的定义进行匹配-->
            <Item sqlType="CHAR|VARCHAR|VARCHAR2" clrType="string"></Item>
            <Item sqlType="NUMBER(1,0)|NUMBER(2,0)" clrType="byte"></Item>
            <Item sqlType="NUMBER(3,0)|NUMBER(4,0)" clrType="short"></Item>
        </Mapping>
    </TypeMapping>

    <!--数据库配置，可以有多个Database节点-->
    <DataSource>
        <!--name:数据库名字；type:数据库类型；templateId:模板id；filterId:过滤器id；directory:实体类生成目录；connString:数据库连接字符串-->
        <Database name="name" type="Oracle" templateId="1" filterId="1" directory="Oracle" connString="*"/>
    </DataSource>
</AutoEntity>
```



#### 其他说明：

1. 为什么会有TypeInTable和TypeInView

   有些数据库版本里视图字段的AllowNull不准(达梦)，为了兼容，TypeInView都是可空的，TypeInTable则是根据AllowNull确定

2. Gbase如何使用

   先在ODBC 数据源管理程序(ODBC Data Sources)配置好用户DSN，然后数据库连接字符串配置对应的DSN名称即可

3. Gbase执行报错，错误消息：在指定的DSN中，驱动程序和应用程序之间的体系结构不匹配

   因为Visual Studio 2019是32位，驱动是64位；或者Visual Studio 2022是64位，驱动是32位。改成2019使用32位驱动；或者2022使用64位驱动

4. 模板里定义`$Table.CamelName()`生成构造函数，实际生成的文件里只有构造函数名，没有括号`()`

   改成`${Table.CamelName}()`即可
