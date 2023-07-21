# Introduction functional programming
Functional programming is a style that emphasizes functions while avoiding state mutation

*Destructive updates*: Updating a value that is stored in memory. The value prior will be overwritten
Listing 1.4: The example appears to be outdated because I do infact get the expected 0,0 result. However, it aims to explain how mutable state is not thread safe because the author
expected to se a random number as the first sum because the Sum operation happens at the same time as the sorting.
The section also explains how immutability can provide us with concurrency for free.

1.2 Advocates for using static imports. Though overuse may cause namespace pollution, reasonable use makes the code more readable.
Also, we should use tuples instead of having a dedicated type just for returning more than one variable.

# 2. Thinking in functions
Mathematical definition: A map between two sets, the domain and the codomain
Statically typed languages: Types represent the sets.
Functions can be represented with:
 * Methods: 
 * Delegates: Type-safe function pointers. Func and Action are delegates. 
	* Arity: refers to the number of arguments
 * Lambda expressions: Declares a function inline.
 * Dictionaries: Provide a direct representation of functions. Associate keys from the domain with values from the codomain
	* Memoization: Allows optimization of computationally expensive functions by storing precomputed values.

## Higher order functions
Functions that take other functions as input or/and return a function as output. Compare and Where are examples of higher order functions.
Can be usefull for deciding how to get date in case of a cache miss.
Adapter function: Returns a modified function. For example with the arguments swaped. Se 2.2.2 Adapter function
Function factories: Takes some static data and returns a function. See 2.2.3 Function factory

### 3. Why function purity matters
Pure functions: Output depends entierly on the input arguments and have no side effects
Impure functions: Factors other than input arguments can affect the output and can have side effects

A function has side effects if it:
 * Mutates global state: Anything outside of function scope. For example private instance fields
 * Mutates its input arguments: 
 * Throws exceptions: It differs depending on whether the function is called in a try-catch
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
`Select` should always be called with pure functions
If we decide to perform it in parallel, internal state will not be as expected because the list will
be chuncked before processed.
We have to decide wether to run it in parallel, the runtime does not know wheter the function is pure or not.
We could parallelize the impure function by using a lock, but that comes at the expence of performance.
Note that static methods can cause problems if they mutate static fields or perform I/O operations. In the latter case
it is testability that's jepordized

Parallel execution will not work for for this formatter because counter is accessed by multiple instances

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
[snippet source]

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
[snippet source]









