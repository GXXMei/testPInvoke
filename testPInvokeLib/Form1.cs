using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;

//https://learn.microsoft.com/zh-cn/dotnet/framework/interop/marshalling-classes-structures-and-unions

//StructLayout 的链接

namespace testPInvokeLib
{


public partial class Form1 : Form
    {
        public delegate bool FPtr(int value);
        public delegate bool FPtr2(string value);

        // StructLayout 允许控制内存中类或结构的数据字段的物理布局
        //应用 StructLayoutAttribute 属性以确保成员在内存中按出现的顺序进行排列。

        //为每个非托管（C++）结构声明托管（C#）结构
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public  struct MyPerson
        {
            public string first;
            public string last;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MyPerson2
        {
            //将对 MyPerson2 类型对象的引用（地址的值）传递到非托管代码
            public IntPtr person;
            public int age;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MyPerson3
        {
            public MyPerson person;
            public int age;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MyArrayStruct
        {
            public bool flag;

            //用于指示数组中的元素个数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public int[] vals;
        }

        //联合体 声明原型
        [StructLayout(LayoutKind.Explicit)]
        public struct MyUnion
        {
            [FieldOffset(0)]
            public int i;
            [FieldOffset(0)]
            public double d;
        }


        [StructLayout(LayoutKind.Explicit, Size = 128)]
        public struct MyUnion2_1
        {
            [FieldOffset(0)]
            public int i;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct MyUnion2_2
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string str;
        }

        //包含整数和字符串的结构数组作为 Out 参数传递到非托管函数
        [StructLayout(LayoutKind.Sequential ,CharSet = CharSet.Ansi)]
        public class MyStruct
        {
            public string buffer;
            public int size;
        }

        //声明带一个指针的结构体
        [StructLayout(LayoutKind.Sequential)]
        public struct MyUnsafeStruct
        {
            public IntPtr buffer;
            public int size;
        }


        //封送不同类型的数组
        //https://learn.microsoft.com/zh-cn/dotnet/framework/interop/marshalling-different-types-of-arrays

        [StructLayout(LayoutKind.Sequential)]
        public struct MyPoint
        {
            public int x;
            public int y;

            public MyPoint(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public  struct MyPerson1
        {
            public string First;
            public string Last;

            public MyPerson1(string first, string last)
            {
                this.First = first;
                this.Last = last;
            }
        }

     public   enum DataType: byte
        {
            DT_I2 = 1,
            DT_I4,
            DT_R4,
            DT_R8,
            DT_STR
        };



        #region 结构体
        [DllImport("PInvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        //引用传递
        public static extern int TestStructInStruct(ref MyPerson2 person2);


        [DllImport("PInvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        //值传递
        public static extern int TestStructInStruct3( MyPerson3 person3);


        [DllImport("PInvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        //引用传递
        public static extern int TestArrayInStruct(ref MyArrayStruct myStruct);
        #endregion


        #region 联合体
        //联合体

        [DllImport("PInvokeLib.dll")]
        internal static extern void TestUnion(MyUnion u, int type);

        [DllImport("PInvokeLib.dll")]
        internal static extern void TestUnion2(MyUnion2_1 u, int type);

        [DllImport("PInvokeLib.dll")]
        internal static extern void TestUnion2(MyUnion2_2 u, int type);
        #endregion


        #region 带有指针和数组的结构体
        //带有指针和数组的结构体
        [DllImport("PInvokeLib.dll")]
        internal static extern void TestOutArrayOfStructs(out int siez ,out IntPtr outArray);

        [DllImport("PInvokeLib.dll")]
        internal static unsafe extern void  TestOutArrayOfStructs(out int siez, MyUnsafeStruct** outArray);
        #endregion


        //封送不同类型的数组
        #region  封送不同类型的数组

        // 为按值排列的整数数组声明托管原型
        // 数组的大小不能改变，但是数组会被复制回来。
        [DllImport("PInvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TestArrayOfInts(
            [In, Out] int[] array, int size);

        // 通过引用为整数数组声明托管原型
        // 数组大小可以改变，但数组不会自动复制回来，因为编译器不知道结果大小
        // 需要手动执行复制
        [DllImport("PInvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TestRefArrayOfInts(
            ref IntPtr array, ref int size);

        // 为按值的整数矩阵声明托管原型。
        [DllImport("PInvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TestMatrixOfInts(
            [In, Out] int[,] pMatrix, int row);

        // 按值声明字符串数组的托管原型。
        [DllImport("PInvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TestArrayOfStrings(
            [In, Out] string[] stringArray, int size);

        // 声明具有整数数组的结构的托管原型。
        [DllImport("PInvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TestArrayOfStructs(
            [In, Out] MyPoint[] pointArray, int size);

        // 声明具有字符串的结构数组的托管原型。
        [DllImport("PInvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TestArrayOfStructs2(
            [In, Out] MyPerson1[] personArray, int size);

        #endregion


        #region 将委托作为回调方法进行封送

        // Declares managed prototypes for unmanaged functions.
        [DllImport("PInvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TestCallBack(FPtr cb, int value);

        [DllImport("PInvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TestCallBack2(FPtr2 cb2, string value);
        #endregion

        #region  声明类
        [DllImport("PInvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateTestClass();

        [DllImport("PInvokeLib.dll", EntryPoint = "?DoSomething@CTestClass@@QAEHH@Z",
            CallingConvention = CallingConvention.ThisCall)]
        public static extern int TestThisCalling(IntPtr ths, int i);

        [DllImport("PInvokeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void  DeleteTestClass(IntPtr ths);
        #endregion

        #region  枚举类型与void*

        [DllImport("PInvokeLib.dll")]
        //当需要传递void*时使用AsAny
        internal static extern void SetData(DataType typ,[MarshalAs(UnmanagedType.AsAny)] Object O);

        //当使用void*时，重载一下该函数

        [DllImport("PInvokeLib.dll", EntryPoint = "SetData")]
        public static extern void SetData2(DataType type, ref double data);

        [DllImport("PInvokeLib.dll", EntryPoint = "SetData")]
        public static extern void SetData2(DataType type, string str);
        #endregion


        public Form1()
        {
            InitializeComponent();

            //结构体
            // UsingStruct();

            //带有数组的结构体
            // UsingArrayStruct();

            //联合体
            //  UsingUnion();

            //带有指针和数组的结构体

            //Console.WriteLine("\nUsing marshal class\n");
            // UsingMarshaling();
            // Console.WriteLine("\nUsing unsafe class\n");
            // UsingUnsafePointer();

            //不同类型数组的调用
            //   DiffTypeOfAyyay();

            //将委托作为回调方法进行封送
            // CallBack();

            //调用类
              UsingClass();

            //枚举类型
           // UsingEnum();

        }

        //枚举类型 加void*
        public static void UsingEnum()
        {
          
            //方法1 指定每个枚举元素
            SetData(DataType.DT_I2, (short)12);

            SetData(DataType.DT_I4, (long)99999);

            SetData(DataType.DT_R4, (float)99.87);
           
            SetData(DataType.DT_R8, (double)1.2345);
            SetData2(DataType.DT_STR, "hello");

            //方法2 仅指定最大长度的值类型和字符串
            double d = 3.14159;

            SetData2(DataType.DT_R8, ref d);
            SetData2(DataType.DT_STR, "abcd");
        }


        //https://learn.microsoft.com/zh-cn/archive/blogs/sanpil/calling-c-unmanaged-class-from-c
     
        //调用类
        public static void UsingClass()
        {
            //获取句柄
            IntPtr cTestClass = CreateTestClass();
            //调用函数
            int res = TestThisCalling(cTestClass, 9);
            Console.WriteLine("\nResult: {0} \n" , res);
            //释放
            DeleteTestClass(cTestClass);
        }

        //结构体
        public static void  UsingStruct()
        {
            //包含带有指向另一结构体指针的结构体
            MyPerson personName;
            personName.first = "Mark";
            personName.last = "Lee";

            //  MyPerson2 内中包含MyPerson
            MyPerson2 personAll;
            personAll.age = 30;

            //IntPtr 类型替换指向非托管结构（如C++）的原始指针
            // AllocCoTaskMem 返回personName的地址(指针)
            IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(personName));

            //StructureToPtr 托管结构的内容(personName)复制到非托管的缓冲区(buffer)
            Marshal.StructureToPtr(personName, buffer, false);
            personAll.person = buffer;

            Console.WriteLine("\nPerson before call: ");
            Console.WriteLine("first = {0}, last = {1}, age = {2}",
                personName.first, personName.last, personAll.age);

            //对 MyPerson2 类型对象的引用（地址的值）传递到非托管(C++)代码
            int res = TestStructInStruct(ref personAll);


            // PtrToStructure  将非托管缓冲区的数据封送到托管对象
            MyPerson personRes = (MyPerson)Marshal.PtrToStructure(personAll.person,
                typeof(MyPerson));

            //FreeCoTaskMem 方法释放非托管(C++)的内存块
            Marshal.FreeCoTaskMem(buffer);

            Console.WriteLine("\nPerson after call: ");
            Console.WriteLine("first = {0}, last = {1}, age = {2}",
                personRes.first, personRes.last, personAll.age);


            //带有嵌套结构体的结构体  即结构体A包含结构体B
            MyPerson3 person3 = new MyPerson3();
            person3.person.first = "John";
            person3.person.last = "Evans";
            person3.age = 27;
            TestStructInStruct3(person3);
        }


        public static void UsingArrayStruct()
        {
            //带有数组的结构体
            MyArrayStruct myStruct = new MyArrayStruct();

            myStruct.flag = false;
            myStruct.vals = new int[3];
            myStruct.vals[0] = 1;
            myStruct.vals[1] = 4;
            myStruct.vals[2] = 9;

            Console.WriteLine("\nStructture with array before call: ");
            Console.WriteLine(myStruct.flag);
            Console.WriteLine("{0}  {1}  {2}", myStruct.vals[0],
                myStruct.vals[1], myStruct.vals[2]);

            //引用传递     
            TestArrayInStruct(ref myStruct);

            Console.WriteLine("\nStructture with array after call: ");
            Console.WriteLine(myStruct.flag);
            Console.WriteLine("{0}  {1}  {2}", myStruct.vals[0],
                myStruct.vals[1], myStruct.vals[2]);
        }

        //联合体示例
        public static void UsingUnion()
        {
            //联合体示例
            MyUnion mu = new MyUnion();
            mu.i = 99;
            TestUnion(mu, 1);

            mu.d = 99.99;
            TestUnion(mu, 2);

            MyUnion2_1 mu2_1 = new MyUnion2_1();
            mu2_1.i = 99;
            TestUnion2(mu2_1, 1);


            MyUnion2_2 mu2_2 = new MyUnion2_2();
            mu2_2.str = "*** string  ***";
            TestUnion2(mu2_2, 2);
        }


        public static void UsingMarshaling()
        {
            int size;
            IntPtr outArray;

            //调用函数
            TestOutArrayOfStructs(out size, out outArray);
            MyStruct[] manArray = new MyStruct[size];
            IntPtr curent = outArray;

            for(int i=0;i <size; i++)
            {
                manArray[i] = new MyStruct();

                //将数据从非托管内存块(curent)封送到托管对象(manArray[i])
                Marshal.PtrToStructure(curent, manArray[i]);

                //释放指定的非托管内存块(curent)所指向的所有子结构
                Marshal.DestroyStructure(curent, typeof(MyStruct));
                curent = (IntPtr)((long)curent + Marshal.SizeOf(manArray[i]));

                Console.WriteLine("Element {0}: {1}  {2}", i, manArray[i].buffer, manArray[i].size);

            }
            //释放由非托管分配的内存块
            Marshal.FreeCoTaskMem(outArray);

        }


        public static unsafe void UsingUnsafePointer()
        {
            int size;
            MyUnsafeStruct* pResut;

            TestOutArrayOfStructs(out size, &pResut);
            MyUnsafeStruct* pCurrent = pResut;
            
            for(int i=0; i<size; i++, pCurrent++)
            {
                Console.WriteLine("Element {0}: {1}  {2}", i,
                    Marshal.PtrToStringAnsi(pCurrent->buffer),
                    pCurrent->size);

                //释放内存
                Marshal.FreeCoTaskMem((IntPtr)pResut);
            }
        }

       
        //不同类型数组的调用
        public static void DiffTypeOfAyyay()
        {
            //array ByVal 值传递
            int[] array1 = new int[10];
            Console.WriteLine(" Interger array passed ByVal before call:");
            for(int i=0; i< array1.Length; i++)
            {
                array1[i] = i;
                Console.Write("  " + array1[i]);
            }

            int sum1 = TestArrayOfInts(array1, array1.Length);
            Console.WriteLine("\nSum1 of elements: " + sum1);
            Console.WriteLine("\nInterger array passed ByVal after call:");
            foreach(int i in array1)
            {
               
                Console.Write("  " + i);
            }


            //array ByRef 引用传递
            int[] array2 = new int[10];
            int size = array2.Length;
            Console.WriteLine("\n\nInteger array passed ByRef before call:");
            for(int i =0; i<array2.Length; i++)
            {
                array2[i] = i;
                Console.Write("  " + array2[i]);
            }

            //分配内存大小
            IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(size) * array2.Length);
            //将array2数组复制到非托管内存指针buffer
            Marshal.Copy(array2, 0, buffer, array2.Length);

            int sum2 = TestRefArrayOfInts(ref buffer, ref size);
            Console.WriteLine("\nSum of elements: " + sum2);

            if(size > 0)
            {
                int[] arrayRes = new int[size];
                //将非托管内存中的指针buffer复制到托管32位有符号整数数组arrayRes中
                Marshal.Copy(buffer, arrayRes, 0, size);

                //释放由非托管COM口任务内存分配器分配的内存块
                Marshal.FreeCoTaskMem(buffer);

                Console.WriteLine("\n\nInteger array passed ByRef after call:");
              foreach(int i in arrayRes)
                {
                    Console.Write("  " + i);
                }
            }else
            {
                Console.WriteLine("\nArray after call is empty");
            }


            // matrix  ByVal  多维数组，通过值传递
            const int DIM = 5;
            int[,] matrix = new int[DIM, DIM];

            Console.WriteLine("\n\nMatrix before call: ");
            for(int i=0; i<DIM; i++)
            {
                for(int j=0; j<DIM; j++)
                {
                    matrix[i, j] = j;
                    Console.Write("  " + matrix[i, j]);
                }
                Console.WriteLine("");
            }

            int sum3 = TestMatrixOfInts(matrix, DIM);
            Console.WriteLine("\nSum of element: " + sum3);
            Console.WriteLine("\nMatrix after call: ");
            for(int i=0;i < DIM; i++)
            {
                for(int j=0; j<DIM; j++)
                {
                    Console.Write("  " + matrix[i, j]);
                }
                Console.WriteLine("");
            }


            // string array ByVal  通过值传递的字符串数组
            string[] strArray = { "one", "two", "three", "four", "five" };
            Console.WriteLine("\n\nstring array before call:");
            foreach(string s in  strArray)
            {
                Console.Write(" " + s);
            }

            int lenSum = TestArrayOfStrings(strArray, strArray.Length);
            Console.WriteLine("\nSum of string lengths: " + lenSum);
            Console.WriteLine("\n\nstring array after call:");
            foreach(string s in strArray)
            {
                Console.Write(" " + s);

            }


            //struct array ByVal 结构体数组 传递值
            MyPoint[] points = { new MyPoint(1, 1), new MyPoint(2, 2), new MyPoint(3, 3) };
            Console.WriteLine("\n\nPoints array before call: ");
            foreach(MyPoint p in points)
            {
                Console.WriteLine($" x = {p.x}, y = {p.y}");
            }

            //调用函数
            int allSum = TestArrayOfStructs(points, points.Length);
            Console.WriteLine("\nSum of points: " + allSum);
            Console.WriteLine("\nPoints array after call: ");
            foreach(MyPoint p in points)
            {
                Console.WriteLine($"x = {p.x}, y = {p.y}");
            }


            // struct with strings array ByVal 字符串结构体数组 传递值
            MyPerson1[] persons =
            {
                new MyPerson1("Kim", "Akers"),
                new MyPerson1("Adam", "Barr"),
                new MyPerson1("Jo", "Brown")
            };

            Console.WriteLine("\n\nPersons array before call:");
            foreach(MyPerson1 pe in persons)
            {
                Console.WriteLine($"First = {pe.First}, Last = {pe.Last}");
            }

            int namesSum = TestArrayOfStructs2(persons, persons.Length);
            Console.WriteLine("\nSum of name lengths: " + namesSum);
            Console.WriteLine("\n\nPersons array after call: ");
            foreach(MyPerson1 pe in persons)
            {
                Console.WriteLine($"First = {pe.First}, Last = {pe.Last}");
            }

        }


        //将委托作为回调方法进行封送
        public static void CallBack()
        {
            FPtr cb = new FPtr(Form1.DoSomething);
            TestCallBack(cb, 99);

            FPtr2 cb2 = new FPtr2(Form1.DoSomething2);
            TestCallBack2(cb2, "abc");
        }

        public static bool DoSomething(int value)
        {
            Console.WriteLine($"\nCallback1 called with param: {value}");
            // ...
            return true;
        }

        public static bool DoSomething2(string value)
        {
            Console.WriteLine($"\nCallback2 called with param: {value}");
            // ...
            return true;
        }

      
    }

}
