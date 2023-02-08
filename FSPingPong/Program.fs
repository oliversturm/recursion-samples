let rec ping c v =
    match c with
    | 0 -> v
    | _ -> pong (c - 1) (v + c)

and pong c v =
    match c with
    | 0 -> v
    | _ -> ping (c - 1) (v + c)

printfn "Ping/pong result: %d" (ping 300000 0)

// let rec timesTwo x = (plus5 x) * 2

// and plus5 x = x + 5



// printfn "Thing: %d" (timesTwo 10)
