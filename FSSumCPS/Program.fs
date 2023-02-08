let rec sumRec =
    function
    | [] -> 0UL
    | x :: xs -> x + (sumRec xs)

let l1: list<uint64> = [ 2UL; 3UL; 6UL; 8UL ]

let outputResult p v = printfn "%s: %d" p v

//printfn "Sum l1: %d" (sumRec l1)

outputResult "sumRec l1" (sumRec l1)

let rec sumRecTail l r =
    match l with
    | [] -> r
    | x :: xs -> (sumRecTail xs (r + x))

outputResult "sumRecTail l1" (sumRecTail l1 0UL)

let rec sumCps l c : unit =
    match l with
    | [] -> c 0UL
    | x :: xs -> sumCps xs (fun ix -> (c (ix + x)))

sumCps l1 (outputResult "sumCps l1")

let longList: list<uint64> = [ 1UL .. 300000UL ]

// This crashes, after 174240 iterations in my test (using Release configuration)
//outputResult "sumRec long" (sumRec longList)

outputResult "sumRecTail long" (sumRecTail longList 0UL)

// This does not crash in the Release configuration. Does in Debug though, good to know.
// I tried it with up to 100 million numbers in the list -- obviously
// the algorithm isn't good on memory consumption, but it does not fail.
//sumCps longList (outputResult "sumCps long")
