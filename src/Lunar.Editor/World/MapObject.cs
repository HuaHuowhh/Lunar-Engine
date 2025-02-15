﻿using System.Drawing.Imaging;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Editor.Utilities;
using Lunar.Graphics;

namespace Lunar.Editor.World
{
    public class MapObject
    {
        private Vector2 _position;
        private Rectangle _collisionRect;
        private Sprite _sprite;
        private long _nextAnimationTime;

        public Sprite Sprite
        {
            get => _sprite;
            set
            {
                _sprite = value;
                _sprite.Transform.Position = this.Position;
                _sprite.Transform.LayerDepth = this.Layer.ZIndex;
                _collisionRect = new Rectangle((int)_position.X, (int)_position.Y,
                    this.Sprite.Transform.Rect.Width, this.Sprite.Transform.Rect.Height);
            }
        }

        public string LuaScriptPath { get; set; }

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;

                if (this.Sprite != null)
                {
                    this.Sprite.Transform.Position = value;
                    _collisionRect = new Rectangle((int)_position.X, (int)_position.Y,
                        this.Sprite.Transform.Rect.Width, this.Sprite.Transform.Rect.Height);
                }
            }
        }

        public bool Animated { get; set; }

        public int FrameTime { get; set; }

        public bool LightSource { get; set; }

        public Color LightColor { get; set; }

        public float LightRadius { get; set; }

        public Layer Layer { get; set; }

        public MapObject(Vector2 position, Layer layer)
        {
            this.Layer = layer;
            this.Position = position;

            this.LightColor = Color.White;
        }

        public void Update(GameTime gameTime)
        {
            if (this.Animated)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds >= _nextAnimationTime)
                {
                    (this.Sprite as AnimatedSprite)?.Next();

                    _nextAnimationTime = (long)gameTime.TotalGameTime.TotalMilliseconds + this.FrameTime;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.Sprite != null)
                spriteBatch.Draw(this.Sprite);
        }

        public bool Contains(Vector2 vector)
        {
            return _collisionRect.Contains(vector);
        }

        public void Save(BinaryWriter bW)
        {
            bW.Write(this.Position.X);
            bW.Write(this.Position.Y);

            if (this.Sprite.Texture.Tag == null || this.Sprite.Texture.Tag.ToString() == "null")
            {
                bW.Write(false);
            }
            else
            {
                bW.Write(true);
                bW.Write(this.Sprite.Texture.Tag.ToString());
                bW.Write(this.Sprite.Transform.Rect.X);
                bW.Write(this.Sprite.Transform.Rect.Y);
                bW.Write(this.Sprite.Transform.Rect.Width);
                bW.Write(this.Sprite.Transform.Rect.Height);
            }

            bW.Write(this.Animated);
            bW.Write(this.FrameTime);
            bW.Write(!string.IsNullOrEmpty(this.LuaScriptPath) ? this.LuaScriptPath : "");

            bW.Write(this.LightSource);
            bW.Write(this.LightRadius);
            bW.Write(this.LightColor.R);
            bW.Write(this.LightColor.G);
            bW.Write(this.LightColor.B);
            bW.Write(this.LightColor.A);
        }

        public static MapObject Load(BinaryReader bR, Layer layer, Project project, TextureLoader textureLoader)
        {
            var position = new Vector2(bR.ReadSingle(), bR.ReadSingle());

            var mapObject = new MapObject(position, layer);

            Texture2D texture;
            Rectangle textureRect;
            if (bR.ReadBoolean())
            {
                string texturePath = bR.ReadString();
                textureRect = new Rectangle(bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32());

                texture = textureLoader.LoadFromFile(project.ClientRootDirectory + "/" + texturePath);
            }
            else
            {
                MemoryStream memStream = new MemoryStream();
                Icons.NullObject.Save(memStream, ImageFormat.Png);

                texture = textureLoader.LoadFromFileStream(memStream);
                textureRect = texture.Bounds;
            }

            mapObject.Animated = bR.ReadBoolean();

            if (mapObject.Animated)
            {
                mapObject.Sprite = new AnimatedSprite(texture);
            }
            else
            {
                mapObject.Sprite = new Sprite(texture);
            }

            mapObject.Sprite.Transform.Position = position;
            mapObject.Sprite.Transform.LayerDepth = layer.ZIndex;
            mapObject.Sprite.Transform.Rect = textureRect;

            mapObject.FrameTime = bR.ReadInt32();
            mapObject.LuaScriptPath = bR.ReadString();

            mapObject.LightSource = bR.ReadBoolean();
            mapObject.LightRadius = bR.ReadSingle();
            mapObject.LightColor = new Color(bR.ReadByte(), bR.ReadByte(), bR.ReadByte(), bR.ReadByte());

            return mapObject;
        }
    }
}