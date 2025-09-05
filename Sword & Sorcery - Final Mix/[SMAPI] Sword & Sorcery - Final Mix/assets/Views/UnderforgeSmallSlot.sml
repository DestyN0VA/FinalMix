<panel layout="content content"
	focusable={^ShouldFocus}
	*context={<>ItemData} >
	<image sprite={@Mods/StardewUI/Sprites/MenuSlotInsetUncolored} 
		tint="#736d6a"
		layout="68px 68px" />
	<lane layout="68px 68px" 
		horizontal-content-alignment="middle" 
		vertical-content-alignment="middle"
		*if={<>^HasItem} >
		<image sprite={this} 
			layout="64px 64px"
			tooltip={^Item}
			+hover:scale="1.05" />
	</lane>
</panel>
