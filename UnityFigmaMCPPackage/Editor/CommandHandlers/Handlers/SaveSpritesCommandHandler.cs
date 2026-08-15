using System;
using UnityFigmaMCP.Common;
using UnityFigmaMCP.Editor.Exporters;

namespace UnityFigmaMCP.Editor
{
    internal sealed class SaveSpritesCommandHandler : ICommandHandler<SaveSpritesCommand, SaveSpritesCommandResult>
    {
        public SaveSpritesCommandResult Handle(ICommandContext context, SaveSpritesCommand command)
        {
            var saveFolder = !string.IsNullOrEmpty(command.SavePath)
                ? command.SavePath
                : context.LayoutSettings.SpritesFolderPath;

            if (string.IsNullOrEmpty(saveFolder))
                throw new Exception("Save path is not set and no default sprites folder configured.");

            var exporter = new SpriteExporter(context.LayoutSettings.SpriteMap);
            var assetPaths = exporter.ExportBatch(saveFolder, command.Sprites);

            var results = new SpriteResult[command.Sprites.Length];
            for (var i = 0; i < command.Sprites.Length; i++)
            {
                results[i] = new SpriteResult
                {
                    AssetPath = assetPaths[i],
                    SpriteName = command.Sprites[i].SpriteName
                };
            }

            return new SaveSpritesCommandResult { Sprites = results };
        }
    }
}
