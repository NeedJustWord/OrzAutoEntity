# OrzAutoEntity

#### 工具名称：

Visual Studio扩展[实体类生成工具](https://github.com/NeedJustWord/OrzAutoEntity)



#### 作用：

读取数据库表和视图结构信息，根据模板生成对应的实体类



#### 支持的数据库类型：

1. Oracle
2. Dm
3. Gbase
4. Sybase



#### 支持的Visual Studio：

1. Visual Studio 2019
2. Visual Studio 2022



#### v1.0.0使用方法：

1. 安装扩展文件
2. 将下面的配置文件保存成 **__entity.xml** 文件名，放到项目文件 ***.csproj** 所在目录
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

    <!--数据库表过滤配置，可以有多个Filter节点-->
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
            <!--数字类型会转换成【类型名(精度/字节数,标度)】的格式参数匹配，是精度还是字节数取决于数据库类型-->
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
        <Database name="name" type="Oracle" templateId="1" filterId="1" directory="Oracle" connString="*" />
    </DataSource>
</AutoEntity>
```



#### 其他说明：

1. 为什么会有TypeInTable和TypeInView

   有些数据库版本里视图字段的AllowNull不准(达梦)，为了兼容，TypeInView都是可空的，TypeInTable则是根据AllowNull确定