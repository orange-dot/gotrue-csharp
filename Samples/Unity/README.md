# GoTrue Unity Notes

The `Gotrue/` source folder now acts as a Unity package root for the auth client in this prototype branch.

Recommended package flow:

1. Add `modules/core-csharp/Core/package.json`
2. Add `modules/gotrue-csharp/Gotrue/package.json`
3. Add `unity/OrangeDot.Supabase.Unity/package.json`

The higher-level Unity sample and composition layer now live in:

- [unity/OrangeDot.Supabase.Unity](/home/dev/orange-dot-supabase-sdk/unity/OrangeDot.Supabase.Unity/README.md)

`UnitySession.cs` stays here as a legacy reference for custom persistence approaches, but the branch now uses `UnitySessionPersistence` from the Unity package instead of the older “copy supporting DLLs by hand” story.
