# Introduction functional programming
Functional programming is a style that emphasizes functions while avoiding state mutation

*Destructive updates*: Updating a value that is stored in memory. The value prior will be overwritten

***Listing 1.4***
: The example appears to be outdated because I do infact get the expected 0,0 result. However, it aims to explain how mutable state is not thread safe because the author
expected to se a random number as the first sum because the Sum operation happens at the same time as the sorting.
The section also explains how immutability can provide us with concurrency for free.

***Section 1.2*** Advocates for using static imports. Though overuse may cause namespace pollution, reasonable use makes the code more readable.
Also, we should use tuples instead of having a dedicated type just for returning more than one variable.

# 2. Thinking in functions
*Mathematical definition*: A map between two sets, the domain and the codomain

*Statically typed languages*: Types represent the sets.
Functions can be represented with:
 * *Methods*: 
 * *Delegates*: Type-safe function pointers. Func and Action are delegates. 
	* *Arity*: refers to the number of arguments
 * *Lambda expressions*: Declares a function inline.
 * *Dictionaries*: Provide a direct representation of functions. Associate keys from the domain with values from the codomain
	* *Memoization*: Allows optimization of computationally expensive functions by storing precomputed values.

## Higher order functions
Functions that take other functions as input or/and return a function as output. Compare and Where are examples of higher order functions.
Can be usefull for deciding how to get date in case of a cache miss.
* *Adapter function*: Returns a modified function. For example with the arguments swaped.
[See 2.2.2](https://github.com/kimrs/functional-programming-in-csharp/blob/c62d378ace864adbe1f1e75a6095e52a1279753b/Functional%20Programming%20in%20CSharp/2-higher-order-functions.cs#L24-L33)
* *Function factories*: Takes some static data and returns a function.
[See 2.2.3](https://github.com/kimrs/functional-programming-in-csharp/blob/c62d378ace864adbe1f1e75a6095e52a1279753b/Functional%20Programming%20in%20CSharp/2-higher-order-functions.cs#L35-L43)

### 3. Why function purity matters
* *Pure functions*: Output depends entierly on the input arguments and have no side effects
* *Impure functions*: Factors other than input arguments can affect the output and can have side effects

A function has side effects if it:
 * Mutates global state, I.E Anything outside of function scope. For example private instance fields
 * Mutates its input arguments 
 * Throws exceptions. Because it differs depending on whether the function is called in a try-catch
 * Performs I/O operations

Benefits of pure functions are:
 * Easy to test
 * Easy to reason about
 * Order of evaluations is not important and can be optimized with
	* Paralellization
	* Lazy evaluation
	* Memoization

#### 3.1.3 Avoid mutating arguments
It forces tight coupling between the caller and callee.
The caller must rely on the method to perform its side effect and the calle relies on the caller to initialize the argument.
Also, changing the class to a struct will radically change behavior because it is then called by value.
Returning a tuple can instead usually avoid this.

### 3.2 Enabling parallelization by avoiding state mutation
`Select` should always be called with pure functions.
If we decide to perform it in parallel, internal state will not be as expected because the list will
be chuncked before processed.
We have to decide wether to run it in parallel because the runtime does not know wheter the function is pure or not.
We could parallelize the impure function by using a lock, but that comes at the expence of performance.
Note that static methods can cause problems if they mutate static fields or perform I/O operations. In the latter case
it is testability that's jepordized

Parallel execution will not work for for the following formatter because counter is accessed by multiple instances

```csharp
class ImpureListFormatter
{
    private int counter;
    
    string PrependCounter(string s) => $"{++counter}. {s}";

    public List<string> Format(List<string> list)
        => list
            .AsParallel()
            .Select(Extensions.ToSentenceCase)
            .Select(PrependCounter)
            .ToList();
}
```
[snippet source](https://github.com/kimrs/functional-programming-in-csharp/blob/8aa650dbf0d580d51c0bbaa50eb65de369345be2/Functional%20Programming%20in%20CSharp/3-why-function-purity-matters.cs#L23-L35)

But it will work for this pure formatter

```csharp
public static class PureListFormatter
{
    public static List<string> Format(List<string> list)
        => list
            .Select(Extensions.ToSentenceCase)
            .Zip(Range(1, list.Count), (s, i) => $"{i}. {s}")
            .ToList();
}
```
[snippet source](https://github.com/kimrs/functional-programming-in-csharp/blob/8aa650dbf0d580d51c0bbaa50eb65de369345be2/Functional%20Programming%20in%20CSharp/3-why-function-purity-matters.cs#L37-L44)

## 3.3 Purity and testability
A function that does I/O operations can never be pure. 
* An api call yields different results any time the remote resoucre changes
* Writing to a file may cause exceptions
* Returning current time yields a different result every time

Some of your code will have to be impure. We can view an impure function as a pure function
that takes the state of the application and the state of the world as arguments.
It this case the **Arrange** step in a test will have to set up the both the explicit and the implicit
inputs to the function under test. This can be done with interfaces and dependency injection.
Because of the amount of boilerplate that is required, the author thinks of it as an anti-pattern.
A better alternative is add `Today` as a parameter to the validator constructor. The validator service
would then have to be transient, because we need a new instance of Today everytime we use it.
But this comes at the expense of performance when the I/O operation is more expensive.
Injecting a function is better. 

```csharp
services.AddSingleton<DateNotPastValidator>(_ => new DateNotPastValidator(() => DateTime.UtcNow.Date));
```

Going one step further and define a delegate and register that would make this clearer.

```csharp
public delegate DateTime Clock();
...
services.AddTransient<Clock>(_ => () => DateTime.UtcNow.Date));
services.AddTransient<DateNotPastValidator>();
```
# Part 2
# 4. Designing function signatures and types
*Arrow notation*: Standard notation for function signatures in the FP community

| Function signature  | C# type               | Example                              |
|---------------------|-----------------------|--------------------------------------|
| `int -> string`     | `Func<int, string>`   | `(int i) => i.ToString()`            | 
| `() -> string`      | `Func<string>`        | `() => "hello"`                      | 
| `int -> ()`         | `Action<int>`         | `(int i) => WriteLine($"gimme {i})"` | 
| `() -> ()`          | `Action`              | `() => WriteLine("Hello")`           | 
| `(int, int) -> int` | `Func<int, int, int>` | `(int a, int b) => a + b`            | 

Higher order functions can also be notated.

```csharp
static R Connect<R>(string connStr, Func<IDbConnection, R> func)
 => ...
```

can be notated as

```csharp
(string, (IDbConnection -> R)) -> R
```

which in C# corresponds to

```csharp
Func<string, Func<IDbConnection, R>, R>
```

* *Honest functions*: Always honors its signature.
I.E no exceptions if validation fails.
And no null values!
We can achieve this with value objects

* *Product types*: Types that are defined by aggregating other types.
* *Sum types*: Types where the possible values are the sum of one or more other types

Model your data objects in a way that gives you fine control over the range of inputs
that your function will need to handle

## 4.3 Modeling the absence of data with Unit
*Unit*: A type that we can use to represent the absence of data without the problems of void
Expressed as an empty tuple

If you write a function that takes a `Func<T>` as argument, and does not use its return value
You would have to write an overload for it if you want to pass a void.

* ***void***: Represents no value
* ***unit***: Represents one value

Unit should be used as a flexible alternative to void
We can use an adapter function to change the type.

```csharp
    public static Func<Unit> ToFunc(this Action action)
        => () => { action(); return default; };
```
[snippet source](https://github.com/kimrs/functional-programming-in-csharp/blob/12c6e223a6a0d9e7c2e0201795ccbbcea566c219/Functional%20Programming%20in%20CSharp/4-designing-function-signatures-and-types.cs#L19-L20)

```csharp
public static class Instrumentation
{
    public static void Time(string op, Action action)
        => Time(op, action.ToFunc());

    public static T Time<T>(string op, Func<T> f)
    {
        var sw = new Stopwatch();
        sw.Start();
        var t = f();
        sw.Stop();
        Console.WriteLine($"{op} took {sw.ElapsedMilliseconds}ms");
        return t;
    }
}
```
[snippet source](https://github.com/kimrs/functional-programming-in-csharp/blob/12c6e223a6a0d9e7c2e0201795ccbbcea566c219/Functional%20Programming%20in%20CSharp/4-designing-function-signatures-and-types.cs#L23C36-L36)


# 5. Modeling the possible absence of data
In C#, whether a function returns null or throws an exception if an illegal argument is used is implementation specific.
We can't know what it does without knowing implementation details. In FP, we combat this by using the Option type.
The option is a container that either wraps a value or no value.

```
option<T> = None | Some
```

For nicer syntax, it is adviced to write a match function for defining paths to execute depending on whether we are dealing with a
Some or a None.

```csharp
public static R Match<T, R>(this Option<T> opt, Func<R> None, Func<T, R> Some)
	=> opt switch
	{
		None<T> => None(),
		Some<T>(var t) => Some(t),
		_ => throw new ArgumentException("Option must be None or Some")
	};
```
[snippet source]()

That way we can easily define paths for both the cases where we have a value and cases where we do not.

```csharp
private static string Greet(Option<string> greetee)
	=> greetee.Match(
		() => "Sorry, who?",
		name => $"Hello, {name}"
	);
```
[snippet source]()

Because Option is a concept, and not an implementation we are free to implement it however we want.
The simplest way would be having an interface with implementations for `Some` and `None` but there are many
things we can do to improve it.
* Using struct for stack allocation and None instead of null as default
* Implementing Deconstruct for easier matching
* Creating convenience methods for Some and None for simpler use
* Implicit operators

## 5.4 Option as the natural result type of partial functions
* *Total functions*: Mappings defined for every element of the domain
* *Partial functions*: Mappings defined for some but not all elements of the domain

A `string -> int` parsing function is partial
because there are infinitely many strings that cannot be parsed to an int.
Partial functions are problematic because it is not clear what a function should do
when given an input for which it can't compute a result. 
Defining it as `string -> Option<int>` however makes it total, because
whichever string we insert will always return a valid Option.
Here is an example of how it can be built.

```csharp
public static Option<int> Parse(string s)
	=> int.TryParse(s, out var result)
		? Some(result)
		: None;
```
[snippet source]()

Value retrieval in C# is inconsistent, dishonest and partial. To fix this we can use adapter functions. 

```csharp
public static Option<string> Lookup(this NameValueCollection collection, string key)
    => collection[key]; // works because of implicit operator

public static Option<T> Lookup<K, T>(this IDictionary<K, T> dictionary, K key)
    => dictionary.TryGetValue(key, out T value) ? Some(value) : None;
```
[snippet source]()


The smart constructor pattern is when you make the constructor of a value object private and instead have a create method that returns an IOption

## 5.5 Dealing with null
The author claims that using Option clearly convey that the data might not be there.
He gives an example where the name of a user is made optional. And instead of having this represented
with null, it is represented with a None. I agree that it forces the developer to take absence into account.
But will this not make the next developer wonder why it is None? Also, I do not see how
None is less ambiguous than null. 

* Enable NRT
* For optionals use Option<T>
* Prevent null from seeping in to the boundaries of your api
  * Throw ArgumentNullException
  * 400 error
  * Convert null to Option

Note to self: Maybe instead of throwing the exception, we should store it in the none. As a because?

# 6 Patterns in functional programming
*Map*: Applies a function to the inner values of a structure. 






