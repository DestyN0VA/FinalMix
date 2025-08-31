<lane orientation="vertical">
	<lane orientation="horizontal" layout="stretch content" horizontal-content-alignment="middle">
		<banner background={@Mods/StardewUI/Sprites/BannerBackground}
			background-border-thickness="48,0"
			padding="12"
			text={#Menus.SkillSelection.Title}/>
	</lane>
	<frame layout="60%[600..] 80%[400..]"
		background={@Mods/StardewUI/Sprites/MenuBackground}
		border={@Mods/StardewUI/Sprites/MenuBorder}
		border-thickness="36, 36, 40, 36"
		padding="15,16,16,16">
		<lane orientation="vertical" 
			layout="stretch">
			<expander layout="stretch content"
				margin="0,0,0,4"
				header-padding="0,12"
				header-background-tint="#99c"
				is-expanded={<>IsExpanded}>
				<label text={#Menus.SkillSelection.ExpanderText}
					font="dialogue"
					*outlet="header"/>
				<label text={#Menus.SkillSelection.Description}/>
			</expander>
			<spacer layout="1px 32px"/>
			<scrollable peeking="64" layout="stretch">
				<lane orientation="vertical">
					<lane orientation="horizontal" 
						layout="stretch content" 
						margin="0, 6, 0, 6" 
						vertical-content-alignment="middle">
						<image layout="80px 80px" sprite={@DN.SnS/SkillSprites/Artificer:Icon}/>
						<spacer layout="32px 32px"/>
						<lane orientation="vertical" layout="stretch content">
							<label text={#Skills.ArtificerSkill.Name} font="dialogue"/>
							<label text={#Skills.ArtificerSkill.MenuDescription}/>
						</lane>
						<spacer layout="32px 32px"/>
						<checkbox is-checked={<>Artificer}/>
					</lane>
					<lane orientation="horizontal" 
						layout="stretch content" 
						margin="0, 6, 0, 6" 
						vertical-content-alignment="middle">
						<image layout="80px 80px" sprite={@DN.SnS/SkillSprites/Bardics:Icon}/>
						<spacer layout="32px 32px"/>
						<lane orientation="vertical" layout="stretch content">
							<label text={#Skills.BardicsSkill.Name} font="dialogue"/>
							<label text={#Skills.BardicsSkill.MenuDescription}/>
						</lane>
						<spacer layout="32px 32px"/>
						<checkbox is-checked={<>Bardics}/>
					</lane>
					<lane orientation="horizontal" 
						layout="stretch content" 
						margin="0, 6, 0, 6" 
						vertical-content-alignment="middle">
						<image layout="80px 80px" sprite={@DN.SnS/SkillSprites/Druidics:Icon}/>
						<spacer layout="32px 32px"/>
						<lane orientation="vertical" layout="stretch content">
							<label text={#Skills.DruidicsSkill.Name} font="dialogue"/>
							<label text={#Skills.DruidicsSkill.MenuDescription}/>
						</lane>
						<spacer layout="32px 32px"/>
						<checkbox is-checked={<>Druidics}/>
					</lane>
					<lane orientation="horizontal" 
						layout="stretch content" 
						margin="0, 6, 0, 6" 
						vertical-content-alignment="middle">
						<image layout="80px 80px" sprite={@DN.SnS/SkillSprites/Paladin:Icon}/>
						<spacer layout="32px 32px"/>
						<lane orientation="vertical" layout="stretch content">
							<label text={#Skills.PaladinSkill.Name} font="dialogue"/>
							<label text={#Skills.PaladinSkill.MenuDescription}/>
						</lane>
						<spacer layout="32px 32px"/>
						<checkbox is-checked={<>Paladin}/>
					</lane>
					<lane orientation="horizontal" 
						layout="stretch content" 
						margin="0, 6, 0, 6" 
						vertical-content-alignment="middle">
						<image layout="80px 80px" sprite={@DN.SnS/SkillSprites/Sorcery:Icon}/>
						<spacer layout="32px 32px"/>
						<lane orientation="vertical" layout="stretch content">
							<label text={#Skills.SorcerySkill.Name} font="dialogue"/>
							<label text={#Skills.SorcerySkill.MenuDescription}/>
						</lane>
						<spacer layout="32px 32px"/>
						<checkbox is-checked={<>Sorcery}/>
					</lane>
				</lane>
			</scrollable>
		</lane>
	</frame>
	<lane orientation="horizontal">
		<spacer layout="32px"/>
		<image layout="content content" 
			focusable="true" 
			sprite={@DN.SnS/UiSprites/Cursors:OKButton} 
			click=|Exit()| 
			+hover:scale="1.1"/>
	</lane>
</lane>