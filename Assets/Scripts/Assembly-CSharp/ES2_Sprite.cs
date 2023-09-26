using System.ComponentModel;
using UnityEngine;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ES2_Sprite : ES2Type
{
	public ES2_Sprite()
		: base(typeof(Sprite))
	{
		key = 31;
	}

	public override void Write(object data, ES2Writer writer)
	{
		Sprite sprite = (Sprite)data;
		writer.Write(sprite.texture);
		writer.Write(sprite.rect);
		float x = (0f - sprite.bounds.center.x) / sprite.bounds.extents.x / 2f + 0.5f;
		float y = (0f - sprite.bounds.center.y) / sprite.bounds.extents.y / 2f + 0.5f;
		writer.Write(new Vector2(x, y));
		writer.Write(sprite.textureRect.width / sprite.bounds.size.x);
		writer.Write(sprite.name);
	}

	public override object Read(ES2Reader reader)
	{
		Sprite sprite = Sprite.Create(reader.Read<Texture2D>(), reader.Read<Rect>(), reader.Read<Vector2>(), reader.Read<float>());
		sprite.name = reader.Read<string>();
		return sprite;
	}
}
