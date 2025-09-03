<lane orientation="vertical"
	horizontal-content-alignment="middle"
	layout="content content" >
	<banner background={@Mods/StardewUI/Sprites/BannerBackground}
		background-border-thickness="48,0"
		padding="12"
		text={#Menus.UnderforgeMenu.Title} />
	<lane orientation="horizontal" vertical-content-alignment="middle">
		<frame layout="content content"
			background={@Mods/StardewUI/Sprites/MenuBackgroundUncolored}
			background-tint="#736d6a"
			border={@Mods/StardewUI/Sprites/MenuBorderUncolored}
			border-thickness="36, 36, 36, 36"
			border-tint="#736d6a"
			padding="16, 16, 16, 16"
			z-index="2" >
			<lane orientation="vertical"
				horizontal-content-alignment="middle"
				*!if={<>Choosing} >
				<lane orientation="vertical"
					horizontal-content-alignment="middle"
					*context={<>Slot}>
					<panel layout="content content"
						focusable="true"
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
					<label text="Input" />
				</lane>
				<spacer layout="16px 32px" />
				<include name="DN.SnS/Views/UnderforgeSmallSlot" 
					*context={<>Preview}/>
				<label text="Preview" />
				<spacer layout="16px 32px" />
				<image sprite={@Mods/StardewUI/Sprites/MenuSlotInsetUncolored}
					focusable={ShouldFocus}
					tint="#98928d" 
					layout="84px 84px"
					+hover:scale="1.1" />
				<label text="Forge" />
			</lane>
			<lane orientation="vertical"
				horizontal-content-alignment="middle"
				vertical-content-alignment="middle"
				layout="content 384px"
				*if={<>Choosing} >
				<include name="DN.SnS/Views/UnderforgeSlot"
					*context={<>Focus} 
					focusable="true" />
			</lane>
		</frame>
		<frame layout="content content"
			background={@Mods/StardewUI/Sprites/MenuBackgroundUncolored}
			background-tint="#736d6a"
			border={@Mods/StardewUI/Sprites/MenuBorderUncolored}
			border-thickness="36, 36, 36, 36"
			border-tint="#736d6a"
			padding="32, 16, 16, 16"
			margin="-48, 0, 0, 0"
			z-index="1" >
			<lane orientation="horizontal"
				horizontal-content-alignment="middle"
				*!if={<>Choosing} >
				<lane orientation="vertical"
					vertical-content-alignment="middle"
					horizontal-content-alignment="middle"
					layout="content content" >
					<include name="DN.SnS/Views/UnderforgeSlot"
						*context={Materia1} />
					<label text="Materia" />
					<spacer layout="1px 24px"/>
					<include name="DN.SnS/Views/UnderforgeSlot"
						*context={Materia2} />
					<label text="Materia" />
					<spacer layout="1px 24px"/>
					<include name="DN.SnS/Views/UnderforgeSlot"
						*context={Materia3} />
					<label text="Materia" />
				</lane>
				<spacer layout="48px 16px"/>
				<lane orientation="vertical"
					vertical-content-alignment="middle"
					horizontal-content-alignment="middle"
					layout="content content" >
					<include name="DN.SnS/Views/UnderforgeSlot"
						*context={Alloy} />
					<label text="Alloy" />
					<spacer layout="84px 24px"/>
					<include name="DN.SnS/Views/UnderforgeSlot"
						*if={IsLLTK}
						*context={Keychain} />
					<label text="Keychain" 
						*if={IsLLTK} />
				</lane>
			</lane>
			<scrollable layout="500px 384px"
				*if={<>Choosing} >
				<grid layout="stretch content"
					item-layout="length: 64+"
					item-spacing="16,16"
					horizontal-item-alignment="middle">
					<image layout="64px 64px" *repeat={<>AvailableItems}
						focusable="true"
						tooltip={Item}
						sprite={ItemData} 
						click=|^ChooseItem(Item)|/>
				</grid>
			</scrollable>
		</frame>
	</lane>
</lane>
