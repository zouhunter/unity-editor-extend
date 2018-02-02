using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections;
using System;
using System.CodeDom;
//using Microsoft.CSharp;
//using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Reflection;
public class NewEditModeTest
{
    [Test]
    public void Test()
    {
        //声明代码的部分
        CodeCompileUnit compunit = new CodeCompileUnit();
        CodeNamespace sample = new CodeNamespace("WorpClasses");
        compunit.Namespaces.Add(sample);
        //引用命名空间
        sample.Imports.Add(new CodeNamespaceImport("System"));//导入System命名空间
        sample.Imports.Add(new CodeNamespaceImport("System.Linq"));//导入System.Linq命名空间
        //在命名空间下添加一个类
        CodeTypeDeclaration wrapProxyClass = new CodeTypeDeclaration("类名");
        //wrapProxyClass.BaseTypes.Add(baseType);// 如果需要的话 在这里声明继承关系 (基类 , 接口)
        wrapProxyClass.CustomAttributes.Add(new CodeAttributeDeclaration("Serializable"));//添加一个Attribute到class上
        sample.Types.Add(wrapProxyClass);//把这个类添加到命名空间 ,待会儿才会编译这个类
        //为这个类添加一个无参构造函数  其实不添加也没事的, 只是做个demo而已
        CodeConstructor constructor = new CodeConstructor();
        constructor.Attributes = MemberAttributes.Public;
        wrapProxyClass.Members.Add(constructor);
        //为这个类添加一个方法   public override int 方法名(string str);
        CodeMemberMethod method = new CodeMemberMethod();
        method.Name = "方法名";
        method.Attributes = MemberAttributes.Override | MemberAttributes.Public;//声明方法是公开 并且override的
        method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "str")); //这个方法添加一个输入参数
        method.ReturnType = new CodeTypeReference(typeof(int));//声明返回值的类型
        method.Statements.Add(new CodeSnippetStatement(" return 1; ")); //方法体里面很简单 直接返回 一个1;

        Microsoft.CSharp. CSharpCodeProvider cprovider = new Microsoft.CSharp.CSharpCodeProvider();
        StringBuilder fileContent = new StringBuilder();
        using (StringWriter sw = new StringWriter(fileContent))
        {
            cprovider.GenerateCodeFromCompileUnit(compunit, sw,new System.CodeDom.Compiler.CodeGeneratorOptions());//想把生成的代码保存为cs文件
        }
    }
}
