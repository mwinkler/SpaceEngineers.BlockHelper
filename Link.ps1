Function ExtractMain($f) {
	
	$lines = Get-Content $f -Encoding utf8

	$wait = $true
	$buffer = @("")

	foreach ($line in $lines) {
		
		# skip empty line
		If ($line.Length -eq 0) {
			continue
		}
		
		# start when main found
		If ($line.ToLower().Contains("void main()")) {
			$wait = $false
		}
		
		# copy to buffer
		If (!$wait) {
			$buffer += $line
		}
	}

	# cut last line
	$buffer = $buffer[0..($buffer.Count - 2)]

	return $buffer
}



$path = "SpaceEngineers.BlockHelper\"

# read source file
$source = $path + "BlockHelper.cs"
$file = Get-Content $source -Encoding utf8

$target= $path + "BlockHelper_Ingame.cs"

# copy
$wait = $true
$buffer = ExtractMain($path + "Examples.cs")
$buffer += "}"

foreach ($line in $file)
{
	If ($line.Length -eq 0) 
	{
		$wait = $false
		continue
	}
	
	
	If (!$wait)
	{
		$buffer += $line
	}
}

$buffer[0..($buffer.Count - 2)] | Out-File $target


#$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
