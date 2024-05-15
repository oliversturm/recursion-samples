// Helper for result output
let outputResult p v = printfn $"%s{p}: %d{v}"

// Sample 1: iterate to a target value, using recursion
let rec go target x =
    if x <> target then go target (x + 1) else x

//outputResult "go 10" (go 10 0)

// Sample 2: use a larger goal
//outputResult "go 300000" (go 300000 0)

// Samples 3-7 from C# -- simply using an actual list sum
// algorithm in F#, since this is a reasonable approach in the language.

// let rec sumRec =
//     function
//     | [] -> 0UL
//     | x :: xs -> x + (sumRec xs)
//
// let l1: list<uint64> = [ 2UL; 3UL; 6UL; 8UL ]

// // Comment the above call to outputResult so the compiler is happy
// // to accept `ulong` as the type for v
// outputResult "sumRec l1" (sumRec l1)

// // Here's a version of the recursive sum function that uses
// // a "calculate first" approach similar to the C# variant.
// let rec sumRecTail l r =
//     match l with
//     | [] -> r
//     | x :: xs -> (sumRecTail xs (r + x))
//
// outputResult "sumRecTail l1" (sumRecTail l1 0UL)

// // In F#, as long as we optimize the recursive function to use
// // a tail call, the compiler usually manages to reshape it into
// // a loop. The following therefore does not crash in F#!
// let longList: list<uint64> = [ 1UL .. 300000UL ]
// outputResult "sumRecTail long" (sumRecTail longList 0UL)

// Bonus: CPS
// let rec sumCps l c : unit =
//     match l with
//     | [] -> c 0UL
//     | x :: xs -> sumCps xs (fun ix -> (c (ix + x)))

// sumCps l1 (outputResult "sumCps l1")

// // This last one works in Release builds, due to compiler TCO
// sumCps longList (outputResult "sumCps long")
