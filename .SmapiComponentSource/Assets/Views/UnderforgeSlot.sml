<panel layout="content content"
	focusable={^ShouldFocus}
	left-click=|^OpenChooseMenu(ID)|
	right-click=|^RemoveItem(ID)|
	*context={<>ItemData} >
	<image sprite={@Mods/StardewUI/Sprites/MenuSlotInsetUncolored} 
		tint="#98928d" 
		layout="84px 84px" />
	<lane layout="84px 84px" 
		horizontal-content-alignment="middle" 
		vertical-content-alignment="middle"
		*if={<>^HasItem} >
		<image sprite={this} 
			layout="80px 80px"
			tooltip={^Item}
			+hover:scale="1.05" />
	</lane>
</panel>
