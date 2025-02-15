﻿/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using Lidgren.Network;

namespace Lunar.Core.Content.Graphics
{
    public class SpriteSheet
    {
        public SpriteInfo Sprite { get; set; }

        public int FrameWidth { get; set; }

        public int FrameHeight { get; set; }

        public virtual int HorizontalFrames => this.Sprite.Transform.Rect.Width / this.FrameWidth;

        public virtual int VerticalFrames => this.Sprite.Transform.Rect.Height / this.FrameHeight;

        public SpriteSheet(SpriteInfo sprite, int frameWidth, int frameHeight)
        {
            this.Sprite = sprite;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
        }

        public NetBuffer Pack()
        {
            var netBuffer = new NetBuffer();
            netBuffer.Write(this.Sprite.TextureName);
            netBuffer.Write(this.FrameWidth);
            netBuffer.Write(this.FrameHeight);
            return netBuffer;
        }
    }
}