New-Item -ItemType Directory -Path "./Plots"

$files = Get-ChildItem "./TemperatureSimulation/bin/Debug/net8.0/plot_*"
foreach ($f in $files) {
    Move-Item -Path $f -Destination "./Plots"
}