using System.Collections;
using System.Collections.Generic;

namespace XCOMModHelper.Patcher
{
    public interface IFilePatcher
    {
        string FileTargetPath { get; }

        List<string> FilesToBackup { get; }

        byte[] FileData { get; }

        void LoadFile();

        PatchState ApplyPatch(byte[] pattern, byte[] patchData);

        void WriteChanges();        
    }
}