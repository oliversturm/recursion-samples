let outputResult p v = printfn $"%s{p}: %d{v}"

let rec go target x =
    if x <> target then go target (x + 1) else x

outputResult "go 300000" (go 300000 0)
