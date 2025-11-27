<scrollable peeking="64" layout="content content">
	<lane>
		<grid layout="40%[200..] 60%[300..]"
			item-layout="length: 64+"
			item-spacing="16,16"
			horizontal-item-alignment="middle"
			*repeat={<>Items}>
			<button>
				<image layout="content content" sprite={~ItemRegistry.GetData(QualifiedItemId)} *if={^ShowSprite(this)}/>
			</button>
		</grid>
	</lane>
</scrollable>