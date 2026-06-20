Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$INVENTOR_EXE  = "C:\Program Files\Autodesk\Inventor 2026\Bin\Inventor.exe"
$AUTOCAD_EXE   = "C:\Program Files\Autodesk\AutoCAD 2025\acad.exe"
$MODELS_FOLDER = "C:\Users\byron\OneDrive\Desktop\ClaudeCodeTest\3d models"

$clrBg      = [Drawing.Color]::FromArgb(26,26,46)
$clrCard    = [Drawing.Color]::FromArgb(22,33,62)
$clrBorder  = [Drawing.Color]::FromArgb(42,42,74)
$clrAccent  = [Drawing.Color]::FromArgb(74,111,165)
$clrAccentFg = [Drawing.Color]::FromArgb(170,196,255)
$clrText    = [Drawing.Color]::White
$clrSub     = [Drawing.Color]::FromArgb(102,102,102)
$clrGreen   = [Drawing.Color]::FromArgb(82,168,82)
$clrRed     = [Drawing.Color]::FromArgb(224,82,82)

# ── FORM ─────────────────────────────────────────────────────────────
$form = New-Object System.Windows.Forms.Form
$form.Text            = "App Launcher"
$form.Size            = New-Object Drawing.Size(460, 520)
$form.StartPosition   = "CenterScreen"
$form.BackColor       = $clrBg
$form.ForeColor       = $clrText
$form.FormBorderStyle = "FixedSingle"
$form.MaximizeBox     = $false
$form.Font            = New-Object Drawing.Font("Segoe UI", 9)

# ── TABS ─────────────────────────────────────────────────────────────
$tabs = New-Object System.Windows.Forms.TabControl
$tabs.Dock      = "Fill"
$tabs.BackColor = $clrBg
$form.Controls.Add($tabs)

$tabLauncher = New-Object System.Windows.Forms.TabPage
$tabLauncher.Text      = "  Launcher  "
$tabLauncher.BackColor = $clrBg
$tabs.Controls.Add($tabLauncher)

$tabParams = New-Object System.Windows.Forms.TabPage
$tabParams.Text      = "  Παράμετροι  "
$tabParams.BackColor = $clrBg
$tabs.Controls.Add($tabParams)

# ── LAUNCHER TAB ─────────────────────────────────────────────────────
function MakeAppButton($parent, $label, $sub, $y) {
    $btn = New-Object System.Windows.Forms.Button
    $btn.Text      = "$label`r`n$sub"
    $btn.Size      = New-Object Drawing.Size(390, 64)
    $btn.Location  = New-Object Drawing.Point(20, $y)
    $btn.BackColor = $clrCard
    $btn.ForeColor = $clrText
    $btn.FlatStyle = "Flat"
    $btn.FlatAppearance.BorderColor = $clrBorder
    $btn.FlatAppearance.MouseOverBackColor = [Drawing.Color]::FromArgb(31,45,90)
    $btn.Font      = New-Object Drawing.Font("Segoe UI", 10)
    $btn.TextAlign = "MiddleLeft"
    $btn.Padding   = New-Object System.Windows.Forms.Padding(12,0,0,0)
    $btn.Cursor    = "Hand"
    $parent.Controls.Add($btn)
    return $btn
}

$lblTitle = New-Object System.Windows.Forms.Label
$lblTitle.Text      = "Επέλεξε εφαρμογή"
$lblTitle.Location  = New-Object Drawing.Point(20, 20)
$lblTitle.Size      = New-Object Drawing.Size(390, 30)
$lblTitle.ForeColor = $clrSub
$lblTitle.Font      = New-Object Drawing.Font("Segoe UI", 9)
$tabLauncher.Controls.Add($lblTitle)

$btnInventor = MakeAppButton $tabLauncher "⚙  Autodesk Inventor 2026" "   Professional CAD" 60
$btnAutocad  = MakeAppButton $tabLauncher "✎  AutoCAD 2025" "   2D & 3D Design" 144

$lblStatus = New-Object System.Windows.Forms.Label
$lblStatus.Text     = ""
$lblStatus.Location = New-Object Drawing.Point(20, 230)
$lblStatus.Size     = New-Object Drawing.Size(390, 20)
$lblStatus.ForeColor = $clrSub
$tabLauncher.Controls.Add($lblStatus)

# ── PARAMS TAB ───────────────────────────────────────────────────────
$pnlTop = New-Object System.Windows.Forms.Panel
$pnlTop.Size     = New-Object Drawing.Size(430, 44)
$pnlTop.Location = New-Object Drawing.Point(8, 8)
$pnlTop.BackColor = $clrBg
$tabParams.Controls.Add($pnlTop)

$btnRefresh = New-Object System.Windows.Forms.Button
$btnRefresh.Text      = "⟳  Ανανέωση"
$btnRefresh.Size      = New-Object Drawing.Size(110, 30)
$btnRefresh.Location  = New-Object Drawing.Point(0, 7)
$btnRefresh.BackColor = $clrCard
$btnRefresh.ForeColor = $clrAccentFg
$btnRefresh.FlatStyle = "Flat"
$btnRefresh.FlatAppearance.BorderColor = $clrAccent
$btnRefresh.Cursor    = "Hand"
$pnlTop.Controls.Add($btnRefresh)

$lblDocName = New-Object System.Windows.Forms.Label
$lblDocName.Text      = "Πάτα Ανανέωση για να φορτωθούν οι παράμετροι"
$lblDocName.Location  = New-Object Drawing.Point(120, 12)
$lblDocName.Size      = New-Object Drawing.Size(300, 20)
$lblDocName.ForeColor = $clrSub
$pnlTop.Controls.Add($lblDocName)

$grid = New-Object System.Windows.Forms.DataGridView
$grid.Size      = New-Object Drawing.Size(430, 380)
$grid.Location  = New-Object Drawing.Point(8, 58)
$grid.BackgroundColor = $clrBg
$grid.ForeColor       = $clrText
$grid.GridColor       = [Drawing.Color]::FromArgb(30,30,56)
$grid.BorderStyle     = "None"
$grid.DefaultCellStyle.BackColor        = $clrCard
$grid.DefaultCellStyle.ForeColor        = $clrText
$grid.DefaultCellStyle.SelectionBackColor = $clrAccent
$grid.DefaultCellStyle.SelectionForeColor = $clrText
$grid.ColumnHeadersDefaultCellStyle.BackColor = [Drawing.Color]::FromArgb(18,17,42)
$grid.ColumnHeadersDefaultCellStyle.ForeColor = $clrSub
$grid.ColumnHeadersDefaultCellStyle.Font = New-Object Drawing.Font("Segoe UI", 8, [Drawing.FontStyle]::Bold)
$grid.ColumnHeadersHeight  = 30
$grid.AlternatingRowsDefaultCellStyle.BackColor = [Drawing.Color]::FromArgb(26,26,46)
$grid.ReadOnly              = $true
$grid.AllowUserToAddRows    = $false
$grid.AllowUserToDeleteRows = $false
$grid.SelectionMode         = "FullRowSelect"
$grid.AutoSizeColumnsMode   = "Fill"
$grid.RowHeadersVisible     = $false
$grid.CellBorderStyle       = "SingleHorizontal"
$tabParams.Controls.Add($grid)

foreach ($col in @(
    @{Name="pName"; Header="Όνομα";  Weight=30},
    @{Name="pVal";  Header="Τιμή";   Weight=40},
    @{Name="pUnit"; Header="Μονάδα"; Weight=15},
    @{Name="pType"; Header="Τύπος";  Weight=15}
)) {
    $c = New-Object System.Windows.Forms.DataGridViewTextBoxColumn
    $c.Name       = $col.Name
    $c.HeaderText = $col.Header
    $c.FillWeight = $col.Weight
    $grid.Columns.Add($c) | Out-Null
}

# ── EVENTS ───────────────────────────────────────────────────────────
$btnInventor.Add_Click({
    if (-not (Test-Path $MODELS_FOLDER)) {
        $lblStatus.ForeColor = $clrRed
        $lblStatus.Text = "Δεν βρέθηκε ο φάκελος '3d models'"
        return
    }
    $files = @(Get-ChildItem -Path $MODELS_FOLDER -File)
    if ($files.Count -eq 0) {
        $lblStatus.ForeColor = $clrRed
        $lblStatus.Text = "Δεν υπάρχουν αρχεία στο φάκελο"
        return
    }

    # File picker dialog
    $picker = New-Object System.Windows.Forms.Form
    $picker.Text            = "Επέλεξε αρχείο"
    $picker.Size            = New-Object Drawing.Size(400, 340)
    $picker.StartPosition   = "CenterParent"
    $picker.BackColor       = $clrCard
    $picker.ForeColor       = $clrText
    $picker.FormBorderStyle = "FixedSingle"
    $picker.MaximizeBox     = $false

    $lbl = New-Object System.Windows.Forms.Label
    $lbl.Text      = "3d models"
    $lbl.Location  = New-Object Drawing.Point(15,14)
    $lbl.Size      = New-Object Drawing.Size(350,18)
    $lbl.ForeColor = $clrSub
    $picker.Controls.Add($lbl)

    $list = New-Object System.Windows.Forms.ListBox
    $list.Size      = New-Object Drawing.Size(358, 210)
    $list.Location  = New-Object Drawing.Point(15, 38)
    $list.BackColor = $clrBg
    $list.ForeColor = $clrText
    $list.Font      = New-Object Drawing.Font("Segoe UI", 10)
    $list.BorderStyle = "None"
    $files | ForEach-Object { $list.Items.Add($_.Name) | Out-Null }
    $list.SelectedIndex = 0
    $picker.Controls.Add($list)

    $btnOk = New-Object System.Windows.Forms.Button
    $btnOk.Text         = "Άνοιγμα"
    $btnOk.Size         = New-Object Drawing.Size(170,36)
    $btnOk.Location     = New-Object Drawing.Point(15, 262)
    $btnOk.BackColor    = $clrCard
    $btnOk.ForeColor    = $clrAccentFg
    $btnOk.FlatStyle    = "Flat"
    $btnOk.FlatAppearance.BorderColor = $clrAccent
    $btnOk.DialogResult = [System.Windows.Forms.DialogResult]::OK
    $picker.Controls.Add($btnOk)
    $picker.AcceptButton = $btnOk

    $btnCnl = New-Object System.Windows.Forms.Button
    $btnCnl.Text         = "Ακύρωση"
    $btnCnl.Size         = New-Object Drawing.Size(170,36)
    $btnCnl.Location     = New-Object Drawing.Point(200, 262)
    $btnCnl.BackColor    = $clrCard
    $btnCnl.ForeColor    = $clrSub
    $btnCnl.FlatStyle    = "Flat"
    $btnCnl.FlatAppearance.BorderColor = $clrBorder
    $btnCnl.DialogResult = [System.Windows.Forms.DialogResult]::Cancel
    $picker.Controls.Add($btnCnl)
    $picker.CancelButton = $btnCnl

    $list.Add_DoubleClick({ $picker.DialogResult = [System.Windows.Forms.DialogResult]::OK; $picker.Close() })

    if ($picker.ShowDialog($form) -eq [System.Windows.Forms.DialogResult]::OK -and $list.SelectedIndex -ge 0) {
        $file = $files[$list.SelectedIndex].FullName
        Start-Process -FilePath $INVENTOR_EXE -ArgumentList "`"$file`""
        $lblStatus.ForeColor = $clrGreen
        $lblStatus.Text = "Άνοιγμα: $($files[$list.SelectedIndex].Name)"
    }
})

$btnAutocad.Add_Click({
    Start-Process -FilePath $AUTOCAD_EXE
    $lblStatus.ForeColor = $clrGreen
    $lblStatus.Text = "Εκκίνηση AutoCAD 2025..."
})

$btnRefresh.Add_Click({
    $grid.Rows.Clear()
    $lblDocName.ForeColor = $clrSub
    $lblDocName.Text = "Φόρτωση..."
    try {
        $inv = [System.Runtime.InteropServices.Marshal]::GetActiveObject("Inventor.Application")
        $doc = $inv.ActiveDocument
        $lblDocName.ForeColor = $clrText
        $lblDocName.Text = $doc.DisplayName

        $modelParams = $doc.ComponentDefinition.Parameters.ModelParameters
        $n = $modelParams.Count
        for ($i = 1; $i -le $n; $i++) {
            try {
                $p = $modelParams.Item($i)
                if (-not $p.IsKey) { continue }
                $expr = if ($p.Expression) { $p.Expression } else { "—" }
                $unit = if ($p.Units)      { $p.Units }      else { "—" }
                $grid.Rows.Add($p.Name, $expr, $unit, "model") | Out-Null
            } catch {}
        }

        if ($grid.Rows.Count -eq 0) {
            $lblDocName.Text = "$($doc.DisplayName)  —  δεν βρέθηκαν model parameters"
        }
    } catch {
        $lblDocName.ForeColor = $clrRed
        $lblDocName.Text = "Το Inventor δεν τρέχει ή δεν υπάρχει ανοιχτό αρχείο"
    }
})

[System.Windows.Forms.Application]::Run($form)
