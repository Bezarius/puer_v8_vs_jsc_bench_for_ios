using System;
using System.Collections.Generic;
using System.Diagnostics;
using Models;
using Puerts;
using UnityEngine;
using UnityEngine.Scripting;

public delegate PlayerProgress GetPlayerProgress();

public delegate void SetPlayerProgress(PlayerProgress playerProgress);

public delegate int Fibonacci(int n);

public class PuertsTests : IScriptTest, IDisposable
{
    private readonly string _moduleName;
    private JsEnv _jsEnv;
    private readonly Stopwatch _sw = new();
    private GetPlayerProgress _getPlayerProgress;
    private SetPlayerProgress _setPlayerProgress;
    private readonly int _iterations;
    private Fibonacci _fibonacci;

    public PuertsTests(string moduleName, int iterations = 1000)
    {
        _moduleName = moduleName;
        _iterations = iterations;
    }

    public void Initialize()
    {
        _jsEnv ??= new JsEnv(new DefaultLoader());
        var module = _jsEnv.ExecuteModule(_moduleName);

        _jsEnv.UsingFunc<PlayerProgress>();
        _getPlayerProgress = module.Get<GetPlayerProgress>("GetPlayerProgress");

        _jsEnv.UsingAction<PlayerProgress>();
        _setPlayerProgress = module.Get<SetPlayerProgress>("SetPlayerProgress");

        _jsEnv.UsingFunc<int, int>();
        _fibonacci = module.Get<Fibonacci>("CalcFib");
    }

    public long TestGetPlayerProgress()
    {
        _sw.Start();
        for (int i = 0; i < _iterations; i++)
        {
            var playerProgress = _getPlayerProgress();
            _sw.Stop();
            playerProgress.AssertPlayerProgressIsValid();
            _sw.Start();
        }

        _sw.Stop();
        var ms = _sw.ElapsedMilliseconds;
        _sw.Reset();
        return ms;
    }

    public long TestSetPlayerProgress()
    {
        _sw.Start();
        for (int i = 0; i < _iterations; i++)
        {
            _sw.Stop();
            var progress = TestDataProvider.GetTestPlayerProgress();
            _sw.Start();
            _setPlayerProgress(progress);
        }

        _sw.Stop();
        var ms = _sw.ElapsedMilliseconds;
        _sw.Reset();
        return ms;
    }

    [Preserve]
    private int _fibResult;
    public long TestFibonacci(int n)
    {
        _sw.Start();
        for (int i = 0; i < _iterations; i++)
        {
            _fibResult = _fibonacci(n);
        }
        var ms = _sw.ElapsedMilliseconds;
        _sw.Reset();
        return ms;
    }

    public void Dispose()
    {
        _jsEnv?.Dispose();
    }
}

[Configure]
public class PuertsTypesBindingConfig
{
    [Binding]
    static IEnumerable<Type> Bindings =>
        new List<Type>
        {
            typeof(List<int>),
            typeof(Type),
            typeof(PlayerProgress),
            typeof(HeroState),
            typeof(Dictionary<int, HeroState>),
            typeof(Dictionary<int, int>),
            typeof(Dictionary<int, long>),
        };

    [Filter]
    static bool FilterMethods(System.Reflection.MemberInfo mb)
    {
        if (mb.DeclaringType == typeof(MonoBehaviour) && mb.Name == "runInEditMode")
        {
            return true;
        }

        if (mb.DeclaringType == typeof(Type) && (mb.Name == "MakeGenericSignatureType" || mb.Name == "IsCollectible"))
        {
            return true;
        }

        if (mb.DeclaringType == typeof(System.IO.File))
        {
            if (mb.Name == "SetAccessControl" || mb.Name == "GetAccessControl")
            {
                return true;
            }
            else if (mb.Name == "Create")
            {
                return true;
            }
        }

        return false;
    }
}