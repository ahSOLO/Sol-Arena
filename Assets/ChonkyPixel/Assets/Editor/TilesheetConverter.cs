using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;
using Object = UnityEngine.Object;

namespace ChonkyPixel
{
    struct SliceData
    {
        public int offsetX;
        public int offsetY;
        public int paddingX;
        public int paddingY;
    }

    public class TilesheetConverter : EditorWindow
    {
        private Object assetFile;
        private string filePath;

        private int tileWidth;
        private int tileHeight;

        private SliceData srcSliceData;
        private SliceData destSliceData;

        private bool createBackup = true;
        private bool overwriteOriginal = true;
        private bool saved;

        private Texture2D sourceTex;
        private Texture2D destTex;

        private Vector2 scrollPosition;
        private Vector2Int tileOrdinals;

        private List<Texture2D> processedTiles = new List<Texture2D>();
        private string message = "";
        private string warning = "";
        private string error = "";

        private float tileThumbWidth = 100;
        private const int maxTiles = 100;

        private const string errorMessage = " Please try again, or contact support (including \r\n" +
                                            "any console log messages) if the problem persists.";

        [MenuItem("Window/2D/Tilesheet converter")]
        public static void ShowWindow()
        {
            GetWindow(typeof(TilesheetConverter), false, "Tilesheet converter");
        }

        // GUI /////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnGUI()
        {
            try
            {
                scrollPosition =
                    EditorGUILayout.BeginScrollView(
                        scrollPosition,
                        GUIStyle.none, GUI.skin.verticalScrollbar);
                ShowImageControls();

                ShowMessages();

                if (processedTiles.Count > 0)
                {
                    Horizontal(GUILayout.MinWidth(240), () =>
                    {
                        EditorGUILayout.ObjectField(
                            "Source image",
                            sourceTex,
                            typeof(Texture2D), true);
                        if (SaveButton())
                        {
                            ExportDestinationFile();
                        }
                    });

                    Vertical(GUILayout.MinWidth(400), () =>
                    {
                        Horizontal(GUILayout.MinWidth(400), () =>
                        {
                            tileThumbWidth = EditorGUILayout.Slider(
                                "Tile thumb size",
                                tileThumbWidth,
                                8, 400);
                        });
                        Horizontal(GUILayout.MinWidth(240),
                            () => { GUILayout.Label("Tile preview", EditorStyles.boldLabel); });
                        DrawPreviewTiles();
                    });
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndScrollView();
            }
            catch (Exception e)
            {
                Error("Error building GUI." + errorMessage);
                Debug.LogException(e);
                throw;
            }
        }

        private bool SaveButton()
        {
            if (saved) return false;

            return GUILayout.Button(overwriteOriginal ? "Overwrite original file" : "Save updated file") &&
                   EditorUtility.DisplayDialog(
                       "Are you sure?",
                       (
                           overwriteOriginal ?
                               "You will overwrite the original file " :
                               "You will create a new file ") +
                           (createBackup ?
                               "and create a backup of the original." :
                               "without any backup."),
                       overwriteOriginal ? "Overwrite" : "Save",
                       "Cancel");
        }

        private void ShowImageControls()
        {
            Vertical(() =>
            {
                GUILayout.Label("Input image", EditorStyles.boldLabel);
                //filePath = EditorGUILayout.TextField("File Path", filePath);
                GUI.SetNextControlName("ObjectDropField");
                assetFile = EditorGUILayout.ObjectField(
                    "Drop image asset",
                    assetFile,
                    typeof(Object), true);

                Horizontal(() =>
                {
                    GUILayout.FlexibleSpace();
                    tileWidth = EditorGUILayout.IntField("Tile Width", tileWidth);
                    GUILayout.FlexibleSpace();
                    tileHeight = EditorGUILayout.IntField("Tile Height", tileHeight);
                    GUILayout.FlexibleSpace();
                });

                Horizontal(() =>
                {
                    GUILayout.FlexibleSpace();
                    srcSliceData.offsetX = EditorGUILayout.IntField("Tile Offset X", srcSliceData.offsetX);
                    GUILayout.FlexibleSpace();
                    srcSliceData.offsetY = EditorGUILayout.IntField("Tile Offset Y", srcSliceData.offsetY);
                    GUILayout.FlexibleSpace();
                });

                Horizontal(() =>
                {
                    GUILayout.FlexibleSpace();
                    srcSliceData.paddingX = EditorGUILayout.IntField("Tile Padding X", srcSliceData.paddingX);
                    GUILayout.FlexibleSpace();
                    srcSliceData.paddingY = EditorGUILayout.IntField("Tile Padding Y", srcSliceData.paddingY);
                    GUILayout.FlexibleSpace();
                });

                Horizontal(() =>
                {
                    GUILayout.FlexibleSpace();
                    createBackup = EditorGUILayout.Toggle("Make backup", createBackup);
                    GUILayout.FlexibleSpace();
                    overwriteOriginal = EditorGUILayout.Toggle("Overwrite original", overwriteOriginal);
                    GUILayout.FlexibleSpace();
                });
            });

            if (GUILayout.Button("Slice"))
            {
                SliceTiles();
            }
        }

        private void DrawPreviewTiles()
        {
            try
            {
                if (tileWidth == 0)
                {
                    Error("Tile width cannot be zero.");
                    return;
                }
                var windowWidth = position.width;
                var tileThumbHeight = (int)((float)tileHeight / (float)tileWidth * (float)tileThumbWidth);
                var tilesWide = (int) Math.Floor(windowWidth / (tileThumbWidth + 10));
                if (tilesWide == 0) tilesWide = 1;
                var tileCount = processedTiles.Count < maxTiles ? processedTiles.Count : maxTiles;
                var tilesHigh = (int) Mathf.Ceil(tileCount / tilesWide) + 1;
                var previewRect = GUILayoutUtility.GetRect(windowWidth, tilesHigh * (tileThumbHeight + 10));
                var count = 0;
                foreach (var tile in processedTiles)
                {
                    var x = count % tilesWide;
                    var y = count / tilesWide;
                    var drawX = previewRect.xMin + x * (tileThumbWidth + 10);
                    var drawY = previewRect.yMin + y * (tileThumbHeight + 10);
                    Debug.Log(drawX + " " + drawY + " " + tileThumbWidth + " " + tileThumbHeight);
                    EditorGUI.DrawPreviewTexture(
                        new Rect(drawX, drawY, tileThumbWidth, tileThumbHeight), tile);
                    count++;
                    if (count >= maxTiles) break;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Error("Error drawing preview tiles. \r\n" + errorMessage);
                throw;
            }
        }

        // Error and message handling //////////////////////////////////////////////////////////////////////////////////

        private void ShowMessages()
        {
            var offset = new RectOffset(10, 10, 5, 5);
            if (error.Length > 0)
            {
                var redStyle = new GUIStyle {normal = {textColor = Color.red}, padding = offset};
                GUILayout.Label(error, redStyle);
            }

            if (warning.Length > 0)
            {
                var magentaStyle = new GUIStyle {normal = {textColor = Color.magenta}, padding = offset};
                GUILayout.Label(warning, magentaStyle);
            }

            if (message.Length > 0)
            {
                var blackStyle = new GUIStyle {normal = {textColor = Color.black}, padding = offset};
                GUILayout.Label(message, blackStyle);
            }
        }

        private void ClearMessages()
        {
            error = "";
            warning = "";
            message = "";
        }

        private void Error(string message)
        {
            if (error.Contains(message)) return;
            error += "\n" + message;
        }

        private void Warning(string message)
        {
            if (warning.Contains(message)) return;
            warning += "\n" + message;
        }

        private void Message(string message)
        {
            if (this.message.Contains(message)) return;
            this.message += "\n" + message;
        }

        // Business logic //////////////////////////////////////////////////////////////////////////////////////////////

        private void FillFilePath()
        {
            filePath = AssetDatabase.GetAssetPath(new SerializedObject(assetFile).targetObject.GetInstanceID());
        }

        private Vector2Int getTileFirstPixel(Vector2Int tilePosition, SliceData sliceData)
        {
            return new Vector2Int
            {
                x = tilePosition.x * tileWidth + sliceData.offsetX + sliceData.paddingX * tilePosition.x,
                y = tilePosition.y * tileHeight + sliceData.offsetY + sliceData.paddingY * tilePosition.y
            };
        }

        private void getTilesPerOrdinal()
        {
            tileOrdinals = Vector2Int.zero;

            var check = new Vector2Int(srcSliceData.offsetX, srcSliceData.offsetY);

            while (check.x < sourceTex.width)
            {
                if (check.x + tileWidth > sourceTex.width) break;
                check.x += tileWidth;
                tileOrdinals.x += 1;
                if (check.x + srcSliceData.paddingX > sourceTex.width) break;
                check.x += srcSliceData.paddingX;
            }

            while (check.y < sourceTex.height)
            {
                if (check.y + tileHeight > sourceTex.height) break;
                check.y += tileHeight;
                tileOrdinals.y += 1;
                if (check.y + srcSliceData.paddingY > sourceTex.height) break;
                check.y += srcSliceData.paddingY;
            }
        }

        private bool ValidateMetrics()
        {
            // Prevent focused field from containing stale data
            GUI.FocusControl("ObjectDropField");

            // Invert negative values
            if (tileHeight < 0) tileHeight = Math.Abs(tileHeight);
            if (tileWidth < 0) tileWidth = Math.Abs(tileHeight);
            if (srcSliceData.offsetX < 0) srcSliceData.offsetX = Math.Abs(srcSliceData.offsetX);
            if (srcSliceData.offsetY < 0) srcSliceData.offsetY = Math.Abs(srcSliceData.offsetY);
            if (srcSliceData.paddingX < 0) srcSliceData.paddingX = Math.Abs(srcSliceData.paddingX);
            if (srcSliceData.paddingY < 0) srcSliceData.paddingY = Math.Abs(srcSliceData.paddingY);

            var ret = true;
            if (tileWidth == 0)
            {
                Error("Tile width is set to zero.");
                ret = false;
            }

            if (tileHeight == 0)
            {
                Error("Tile height is set to zero.");
                ret = false;
            }

            if (!ret) Error("Please fix any issues and try again.");
            return ret;
        }

        private void BuildTileList()
        {
            try
            {
                var tilePosition = new Vector2Int();
                for (tilePosition.x = 0; tilePosition.x < tileOrdinals.x; tilePosition.x++)
                {
                    for (tilePosition.y = 0; tilePosition.y < tileOrdinals.y; tilePosition.y++)
                    {
                        var pos = getTileFirstPixel(tilePosition, srcSliceData);
                        var tex = new Texture2D(tileWidth, tileHeight, TextureFormat.ARGB32, false);
                        var cols = sourceTex.GetPixels(pos.x, pos.y, tileWidth, tileHeight);
                        tex.SetPixels(0, 0, tileWidth, tileHeight, cols);
                        tex.filterMode = FilterMode.Point;
                        tex.Apply();
                        processedTiles.Add(tex);
                    }
                }
            }
            catch (Exception e)
            {
                Error("Error building tile list." + errorMessage);
                Debug.LogException(e);
                throw;
            }
        }

        private void ClearTileList()
        {
            foreach (var tile in processedTiles) DestroyImmediate(tile);
            processedTiles = new List<Texture2D>();
        }

        private void ReadSourceImage()
        {
            var imageData = File.ReadAllBytes(filePath);

            sourceTex = new Texture2D(2, 2, TextureFormat.ARGB32, false);

            sourceTex.LoadImage(imageData);
        }

        private void CreateDestinationTexture()
        {
            SetDestSliceData();
            var destWidth = destSliceData.offsetX + tileOrdinals.x * tileWidth +
                            (tileOrdinals.x - 1) * destSliceData.paddingX + 1;
            var destHeight = destSliceData.offsetY + tileOrdinals.y * tileHeight +
                             (tileOrdinals.y - 1) * destSliceData.paddingY + 1;

            destTex = new Texture2D(destWidth, destHeight, TextureFormat.ARGB32, false);
        }

        private void SetDestSliceData()
        {
            var reSlice = false;
            destSliceData = srcSliceData;
            if (destSliceData.offsetX < 1)
            {
                destSliceData.offsetX = 1;
                Warning("Output X Offset increased to 1.");
                reSlice = true;
            }

            if (destSliceData.offsetY < 1)
            {
                destSliceData.offsetY = 1;
                Warning("Output Y Offset increased to 1.");
                reSlice = true;
            }

            if (destSliceData.paddingX < 2)
            {
                destSliceData.paddingX = 2;
                Warning("Output X Padding increased to 2.");
                reSlice = true;
            }

            if (destSliceData.paddingY < 2)
            {
                destSliceData.paddingY = 2;
                Warning("Output Y Padding increased to 2.");
                reSlice = true;
            }

            if (reSlice)
                Message("If the image has previously been sliced, please " +
                        "re-slice with any\nnew values after fixing the image.");
        }

        private void DrawDestTile(Vector2Int pos, Texture2D tex)
        {
            var positions = new Vector2Int[]
            {
                // Corners first
                new Vector2Int(pos.x - 1, pos.y - 1),
                new Vector2Int(pos.x + 1, pos.y - 1),
                new Vector2Int(pos.x - 1, pos.y + 1),
                new Vector2Int(pos.x + 1, pos.y + 1),
                // Then sides
                new Vector2Int(pos.x - 1, pos.y),
                new Vector2Int(pos.x + 1, pos.y),
                new Vector2Int(pos.x, pos.y + 1),
                new Vector2Int(pos.x, pos.y - 1),
                // Then the middle last
                pos
            };
            var cols = tex.GetPixels(0, 0, tileWidth, tileHeight);
            foreach (var loc in positions)
            {
                destTex.SetPixels(loc.x, loc.y, tileWidth, tileHeight, cols);
                destTex.Apply();
            }
        }

        private void BuildDestinationTiles()
        {
            try
            {
                var i = 0;
                var tilePosition = new Vector2Int();
                for (tilePosition.x = 0; tilePosition.x < tileOrdinals.x; tilePosition.x++)
                {
                    for (tilePosition.y = 0; tilePosition.y < tileOrdinals.y; tilePosition.y++)
                    {
                        var pos = getTileFirstPixel(tilePosition, destSliceData);

                        var tex = processedTiles[i];
                        i += 1;
                        DrawDestTile(pos, tex);
                    }
                }
            }
            catch (Exception e)
            {
                Error("Error building destination tileset." + errorMessage);
                Debug.LogException(e);
                throw;
            }
        }

        private void ExportDestinationFile()
        {
            try
            {
                if (createBackup)
                {
                    FileUtil.DeleteFileOrDirectory(filePath + ".backup.png");
                    FileUtil.CopyFileOrDirectory(filePath, filePath + ".backup.png");
                    Message("Backup created.");
                }

                var bytes = destTex.EncodeToPNG();
                if (overwriteOriginal)
                    FileUtil.DeleteFileOrDirectory(filePath);

                File.WriteAllBytes(overwriteOriginal ? filePath : filePath + ".update.png", bytes);
                Message(overwriteOriginal ? "Original file overwritten." : "Updated file saved.");
                saved = true;
            }
            catch (Exception e)
            {
                Error("Error exporting destination file.\r\n" + errorMessage);
                Debug.LogException(e);
                throw;
            }
        }

        private void SliceTiles()
        {
            try
            {
                saved = false;
                ClearMessages();
                ClearTileList();
                if (!ValidateMetrics()) return;

                FillFilePath();
                ReadSourceImage();
                getTilesPerOrdinal();
                BuildTileList();

                CreateDestinationTexture();
                BuildDestinationTiles();
            }
            catch (Exception e)
            {
                Error("An error occurred in the slice process." + errorMessage);
                Debug.LogException(e);
                throw;
            }
        }

        // GUI sugar ///////////////////////////////////////////////////////////////////////////////////////////////////

        private static void Horizontal(GUILayoutOption layout, Action contents)
        {
            try
            {
                EditorGUILayout.BeginHorizontal(layout);
                contents.Invoke();
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        private static void Horizontal(Action contents)
        {
            try
            {
                EditorGUILayout.BeginHorizontal();
                contents.Invoke();
            }
            finally{
                EditorGUILayout.EndHorizontal();
            }
        }

        private static void Vertical(GUILayoutOption layout, Action contents)
        {
            try
            {
                EditorGUILayout.BeginVertical(layout);
                contents.Invoke();
            }
            finally
            {
                EditorGUILayout.EndVertical();
            }
        }

        private static void Vertical(Action contents)
        {
            try
            {
                EditorGUILayout.BeginVertical();
                contents.Invoke();
            }
            finally
            {
                EditorGUILayout.EndVertical();
            }
        }
    }
}
