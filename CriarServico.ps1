$serviceName = "OrdemServicos"
$publishPath = "C:\Sites\OrdemServicos"
$dllName = "Ordem_Servicos_Web.dll"
$dotnetPath = "C:\Program Files\dotnet\dotnet.exe"

$binPath = "`"$dotnetPath`" `"$publishPath\$dllName`""

# Cria o serviço
sc.exe create $serviceName binPath= $binPath start= auto

# Define descrição opcional
sc.exe description $serviceName "Serviço ASP.NET Core - Ordem de Serviços"

# Inicia o serviço
sc.exe start $serviceName