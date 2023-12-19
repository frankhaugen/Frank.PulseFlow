# Style Guide

## Introduction

This document describes the coding style used in this repository. It is based on SOLID principles, and should emphasize Dependency Injections, and the Single Responsibility Principle.

## Table of Contents

## Naming Conventions

| Token          | Convention  | Grammar Terms   | Example           |
|----------------|-------------|-----------------|-------------------|
| Namespace      | PascalCase  | Noun            | `Frank.PulseFlow` |
| Class          | PascalCase  | Noun            | `PulseFlow`       |
| Interface      | IPascalCase | Noun            | `IPulseFlow`      |
| Enum           | PascalCase  | Noun            | `PulseFlowState`  |
| Enum Value     | PascalCase  | Noun, Adjective | `Running`         |
| Method         | PascalCase  | Verb            | `Start`           |
| Property       | PascalCase  | Noun            | `State`           |
| Field          | _camelCase  | Noun            | `_state`          |
| Parameter      | camelCase   | Noun            | `state`           |
| Local Variable | camelCase   | Noun            | `state`           |
| Constant       | PascalCase  | Noun            | `Running`         |
| Event          | PascalCase  | Noun            | `Started`         |
| Delegate       | PascalCase  | Noun            | `Started`         |
| Type Parameter | PascalCase  | Noun            | `TKey`            |
| Type Argument  | PascalCase  | Noun            | `string`          |

## General Guidelines

### Use `var` when possible

Use `var` when the type of the variable is obvious from the right-hand side of the assignment, (e.g. `var x = 1;` is fine, but `var x = new Foo();` is not).

### Use `nameof` when possible

Use `nameof` when the name of the variable is needed, (e.g. `nameof(foo)` instead of `"foo"`).

### Use `const` when possible

Use `const` when the value of the variable is known at compile time, (e.g. `const int x = 1;` is fine, but `const int x = Foo();` is not).

### Use `readonly` when possible

Use `readonly` when the value of the variable is known at compile time, and the variable is not a constant, (e.g. `readonly int x = 1;` is fine, but `readonly int x = Foo();` is not).

### Use `static` when possible

Use `static` when the value of the variable is known at compile time, and the variable is not a constant, (e.g. `static int x = 1;` is fine, but `static int x = Foo();` is not).

### Use Expression-Bodied Members when possible

Use Expression-Bodied Members when the body of the member is a single expression, (e.g. `public int Foo() => 1;` is fine, but `public int Foo() { return 1; }` is not).

## Class Guidelines

### Use `sealed` when possible

Use `sealed` when the class is not intended to be inherited from, (e.g. `sealed class Foo { }` is fine, but `class Foo { }` is not).

### Use `static` when possible

Use `static` when the class is not intended to be instantiated, (e.g. `static class Foo { }` is fine, but `class Foo { }` is not).

## Helper Class Guidelines

### Use `static` always

All helper classes should be `static`, (e.g. `static class Foo { }` is fine, but `class Foo { }` is not).

### Helpers should be `internal` or `private` unless they are intended to be used outside of the assembly

All helper classes should be `internal` or `private` unless they are intended to be used outside of the assembly, (e.g. `internal static class Foo { }` is fine, but `public static class Foo { }` is not).

When a helper class is intended to be used outside of the assembly, it should be `public`, (e.g. `public static class Foo { }` is fine, but `internal static class Foo { }` is not). This is rare, and should be avoided if possible, but something like a DateTime helper class that expose Week based operations might be useful to other assemblies.

## Interface Guidelines

### Use `I` prefix always

All interfaces should be prefixed with `I`, (e.g. `IPulseFlow` is fine, but `PulseFlow` is not).

### Use `internal` or `public` always

All interfaces should be `internal` or `public`, (e.g. `internal interface IPulseFlow { }` is fine, but `private interface IPulseFlow { }` is not).

## Enum Guidelines

### Use `PascalCase` always

All enums should be `PascalCase`, (e.g. `enum PulseFlowState { Running, Stopped }` is fine, but `enum PulseFlowState { running, stopped }` is not).

### Don't suffix with `Enum`, 'Type', or `Flag`

Don't suffix with `Enum`, 'Type', or `Flag`, (e.g. `enum PulseFlowState { Running, Stopped }` is fine, but `enum PulseFlowStateEnum { Running, Stopped }` is not).

### Don't use `Flags` unless the enum is intended to be used as a set of flags

Don't use `Flags` unless the enum is intended to be used as a set of flags, (e.g. `enum PulseFlowState { Running, Stopped }` is fine, but `enum PulseFlowState { Running = 1, Stopped = 2 }` is not).

### Use `internal` or `public` always

All enums should be `internal` or `public`, (e.g. `internal enum PulseFlowState { Running, Stopped }` is fine, but `private enum PulseFlowState { Running, Stopped }` is not).

## Enum Value Guidelines

### Use `PascalCase` always

All enum values should be `PascalCase`, (e.g. `enum PulseFlowState { Running, Stopped }` is fine, but `enum PulseFlowState { running, stopped }` is not).





