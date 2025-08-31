<frame layout="853px content"
	background={@Mods/StardewUI/Sprites/MenuBackgroundUncolored}
	background-tint="#eba867"
	border={@Mods/StardewUI/Sprites/MenuBorder}
	border-thickness="36, 36, 40, 36"
	padding="16,16,16,16">
	<lane orientation="vertical">
		<lane orientation="horizontal"
			horizontal-content-alignment="middle"
			layout="stretch content">
			<banner layout="content content" text={#Menus.LevelUpMenu.Both}/>
		</lane>
		<label text={:SharedInfo}/>
	</lane>
</frame>