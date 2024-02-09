## WHAT is PuerTS (PUER Typescript)?

`PuerTS` is a TypeScript programming solution in Unity/Unreal/DotNet.

*   rovides a JavaScript Runtime.
*   allows TypeScript to access the host engine with the help of TypeScript declarations generation.

## WHY should I use PuerTS?

* Facilitates game-building processes by combining JavaScript/Node.js ecosystem and professional game engines
*   In contrast to Lua script, TypeScript supports static type checking, which significantly improves code robustness and maintainability.
*   High efficiency: supports reflection call throughout the host - no extra steps needed for interop with C++/C#.
*   High performance: supports static wrapper generation - handles complex scenes with high-performance demands.
*   Talented WebGL Support: massive advantage in performance and dev efficiency compared to Lua, even faster than pure C# in some cases.

## Getting Started with PuerTS

Prepare a scene and a MonoBehaviour component in Unity, and write the following code in the MonoBehaviour:

```csharp
//1. Hello World  
void Start() {  
    Puerts.JsEnv env = new Puerts.JsEnv();  
    env.Eval(@"  
        console.log('hello world');  
    ")  
}  
```

After execution, you will see `Hello world` in the Unity console.

That's how easy it is with PuerTS!

## Calling C# from JavaScript

In the previous example, we briefly tried Hello World:

```csharp
//1. Hello World  
void Start() {  
    Puerts.JsEnv env = new Puerts.JsEnv();  
    env.Eval(@"  
        console.log('hello world');  
    ");  
}  
```

In fact, the `console.log` here is not quite the same as the one in the browser. This `console.log` is intercepted by PuerTS and will actually call `UnityEngine.Debug.Log` to print the string content.

With the help of Puerts, the integration between JavaScript and C# can be even more exciting. See below for more:

***

### Object creating

```csharp
//2. Creating a C# object  
void Start() {  
    Puerts.JsEnv env = new Puerts.JsEnv();  
    env.Eval(@"  
        console.log(new CS.UnityEngine.Vector3(1, 2, 3));  
        // (1.0, 2.0, 3.0)  
    ");  
}  
```

In this example, we directly created a C# Vector in JavaScript!

In the Javascript environment created by PuerTS, you can access any C# class by entering the FullName (the complete namespace path) of any class, including creating a Vector3 object directly, using the CS object.

Of course, it is still cumbersome to write out the full namespace, but you can also simplify it by declaring a variable alias:

```javascript
    const Vector2 = CS.UnityEngine.Vector2;  
    console.log(Vector2.one)  
```

***

### properties accessing

Once the object is created, calling its methods or accessing its properties is also very easy.

```csharp
//3. Calling C# functions or object methods  
void Start() {  
    Puerts.JsEnv env = new Puerts.JsEnv();  
    env.Eval(@"  
        CS.UnityEngine.Debug.Log('Hello World');  
        const rect = new CS.UnityEngine.Rect(0, 0, 2, 2);  
        CS.UnityEngine.Debug.Log(rect.Contains(CS.UnityEngine.Vector2.one)); // True  
        rect.width = 0.1  
        CS.UnityEngine.Debug.Log(rect.Contains(CS.UnityEngine.Vector2.one)); // False  
    ");  
}  
```

## As you can see, whether it's function calls or property access/assignment, the usage is exactly the same as in C#.

### special calls

However, there are still some usage in C# that are not commonly seen in JS, such as **ref**, **out**, and **generics**. We need to use the API provided by PuerTS to implement them.

```csharp
//4. out/ref/generics  
class Example4 {  
    public static double InOutArgFunc(int a, out int b, ref int c)  
    {  
        Debug.Log("a=" + a + ",c=" + c);  
        b = 100;  
        c = c * 2;  
        return a + b;  
    }  
}  
void Start() {  
    Puerts.JsEnv env = new Puerts.JsEnv();  
    env.Eval(@"  
        // create a variable that can be used for out/ref parameters through puer.$ref  
        let p1 = puer.$ref();  
        let p2 = puer.$ref(10);  
        let ret = CS.Example4.InOutArgFunc(100, p1, p2);  
        console.log('ret=' + ret + ', out=' + puer.$unref(p1) + ', ref=' + puer.$unref(p2));  
        // ret=200, out=100, ref=20  
  
        // create a List<int> type through puer.$generic  
        let List = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);  
        let lst = new List();  
        lst.Add(1);  
        lst.Add(0);  
        lst.Add(2);  
        lst.Add(4);  
    ");  
}  
```

done easily!

>  It should be noted that you may think, "Typescript supports generics, why not use them?" Unfortunately, TypeScript generics are only a compile-time concept. At runtime, JavaScript is still running, so puer.\$generic is still needed to handle them.

***

### typeof and operator overload

In addition to these special usages, there are two more situations to introduce: typeof function and operator overload:

```csharp
//5. typeof/operator overload  
void Start() {  
    Puerts.JsEnv env = new Puerts.JsEnv();  
    env.Eval(@"  
        let go = new CS.UnityEngine.GameObject('testObject');  
        go.AddComponent(puer.$typeof(CS.UnityEngine.ParticleSystem));  
  
        const Vector3 = CS.UnityEngine.Vector3;  
        let ret = Vector3.op_Multiply(Vector3.up, 1600)  
          
        console.log(ret) // (0.0, 1600.0, 0.0)  
    ");  
}  
```

Because C#'s `typeof` cannot be accessed through C# namespaces and plays a role similar to keywords, PuerTS provides the built-in method `$typeof` for access.

Furthermore, because JS has not fully supported operator overload yet (TC39 is still in the proposal stage), op\_xxxx needs to be used instead of the operator.

***

### async

Let's look at the last case of JS calling C#: async.

```csharp
// async  
class Example6 {  
    public async Task<int> GetFileLength(string path)  
    {  
        Debug.Log("start read " + path);  
        using (StreamReader reader = new StreamReader(path))  
        {  
            string s = await reader.ReadToEndAsync();  
            Debug.Log("read " + path + " completed");  
            return s.Length;  
        }  
    }  
}  
  
void Start() {  
    Puerts.JsEnv env = new Puerts.JsEnv();  
    env.Eval(@"  
        (async function() {  
            let task = obj.GetFileLength('xxxx');  
            let result = await puer.$promise(task);  
            console.log('file length is ' + result);  
        })()  
        .catch(err=> {  
            console.error(err)  
        })  
    ");  
}  
```

For C#'s async function, on the JS side, by wrapping the task returned by C# with puer.\$promise, we can use await to call it.

## Invoking JavaScript in C\#

### Invoking via Delegate

Puerts provides a crucial ability to convert JavaScript functions into C# delegates. With this ability, you can invoke JavaScript from the C# side.

```csharp
public class TestClass  
{  
    Callback1 callback1;  
  
    public delegate void Callback1(string str);  
  
    public void AddEventCallback1(Callback1 callback1)  
    {  
        this.callback1 += callback1;  
    }  
    public void Trigger()  
    {  
        if (callback1 != null)   
        {  
            callback1("test");  
        }  
    }  
}  
  
void Start() {  
    Puerts.JsEnv env = new Puerts.JsEnv();  
    env.Eval(@"  
        const obj = new CS.TestClass();  
        obj.AddEventCallback1(i => console.log(i));  
        obj.Trigger();  
        // Printed the obj variable  
        // Although triggered from JS, it is actually calling JS functions from C#, completing the console.log  
    ");  
}  
```

### Passing Arguments from C# to JavaScript

When converting a JS function to a delegate, you can also convert it to a delegate with parameters, so you can pass any C# variables to JavaScript. The rules for type conversion when passing arguments are the same as when returning variables from C# to JavaScript.

```csharp
void Start() {  
    Puerts.JsEnv env = new Puerts.JsEnv();  
    // Here, you can directly get the delegate from the result of Eval  
    System.Action<int> LogInt = env.Eval<System.Action<int>>(@"  
        const func = function(a) {  
            console.log(a);  
        }  
        func;  
    ");  
  
    LogInt(3); // 3  
}  
```

>  Note that if the delegate you generated has value type parameters, you need to add a UsingAction or UsingFunc declaration. See FAQ for details.

### Invoking JavaScript from C# and Getting Return Values

Similar to the previous section, just need to convert the Action delegate to a Func delegate.

```csharp
void Start() {  
    Puerts.JsEnv env = new Puerts.JsEnv();  
    // Here, you can directly get the delegate from the result of Eval  
    System.Func<int, int> Add3 = env.Eval<System.Func<int, int>>(@"  
        const func = function(a) {  
            return 3 + a;  
        }  
        func;  
    ");  
  
    System.Console.WriteLine(Add3(1)); // 4  
}  
```

>  Note that if the delegate you generated has value type parameters, you need to add a UsingAction or UsingFunc declaration. See FAQ for details.

***

### Implementing MonoBehaviour in JS

With all the abilities mentioned above, we can easily implement this functionality in JS.

```csharp
using System;  
using Puerts;  
using UnityEngine;  
  
public class JsBehaviour : MonoBehaviour  
{  
    public Action JsStart;  
    public Action JsUpdate;  
    public Action JsOnDestroy;  
  
    static JsEnv jsEnv;  
  
    void Awake()  
    {  
        if (jsEnv == null) jsEnv = new JsEnv(new DefaultLoader(), 9229);  
  
        var init = jsEnv.Eval<ModuleInit>(@"const m =   
            class Rotate {  
                constructor(bindTo) {  
                    this.bindTo = bindTo;  
                    this.bindTo.JsUpdate = () => this.onUpdate();  
                    this.bindTo.JsOnDestroy = () => this.onDestroy();  
                }  
                  
                onUpdate() {  
                    console.log('update...')  
                }  
                  
                onDestroy() {  
                    console.log('onDestroy...');  
                }  
            }  
  
            (function(bindTo) {  
                new Rotate(bindTo);  
            })  
        ");  
  
        if (init != null) init(this);  
    }  
  
    void Start()  
    {  
        if (JsStart != null) JsStart();  
    }  
  
    void Update()  
    {  
        jsEnv.Tick();  
        if (JsUpdate != null) JsUpdate();  
    }  
  
    void OnDestroy()  
    {  
        if (JsOnDestroy != null) JsOnDestroy();  
        JsStart = null;  
        JsUpdate = null;  
        JsOnDestroy = null;  
    }  
}  
```

This feature has been implemented by many enthusiastic members of the community, and you can happily choose from their implementations.

## Further reading

for more document, see <https://puerts.github.io>
