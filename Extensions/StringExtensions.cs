using TCGShopBetterPackOpener.Globals;

namespace TCGShopBetterPackOpener.Extensions;

public static class StringExtensions
{
    public static Vector3 StringToVector(this string vectorString)
    {
        var parts = vectorString.Split(' ');
        if (parts.Length != 3)
        {
            throw new FormatException($"Invalid vector string format: {vectorString}");
        }
        return new Vector3(float.Parse(parts[0], NFP.Default), float.Parse(parts[1], NFP.Default), float.Parse(parts[2], NFP.Default));
    }
}