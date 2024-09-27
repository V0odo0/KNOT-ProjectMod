![Unity version](https://img.shields.io/badge/Unity-2021.3%2B-blue)
![Platforms](https://img.shields.io/badge/platforms-all-blue)

![KnotProjectMod_icon](https://github.com/user-attachments/assets/d826d816-566f-4184-8bc7-e3e583cb28dc)

**[Changelog](https://github.com/V0odo0/KNOT-ProjectMod/blob/main/CHANGELOG.md)**

KnotProjectMod adds a set of tools for automating certain processes that require user intervention.

For example, if you need to modify a set of packages for different platforms, you can create a Preset that will sequentially add or remove multiple packages for you. The Mod chain will continue executing even after assembly reload.

> [!WARNING]  
> Use this tool with caution, as certain actions may harm your project.

## Installation

Add `com.knot` Scoped Registry from `https://registry.npmjs.com` in `Project/Package Manager`

![image](https://github.com/user-attachments/assets/ca20c30a-3ac3-494b-9e44-690630faf9db)

Open `Window/Package Manager` and install package from `Packages: My Registries`

![image](https://github.com/user-attachments/assets/8e552498-7faa-4996-87a0-2a784679ee87)


## Use Case #1

Create new ProjectMod Preset Asset

![image](https://github.com/user-attachments/assets/8844d36f-cdac-4b0a-9f7f-dc78df612a9e)

Setup Mod Chain and click Start

![image](https://github.com/user-attachments/assets/46c6478e-d955-4846-a239-6465fce63017)

Create Additional Preset for reverting changes if required

![image](https://github.com/user-attachments/assets/0bc9be3f-7435-4372-8c8a-8387fbfea16b)

## Creating custom Mod

Place your Project Mod script under /Editor folder. Deriving from IKnotModAction allows you to select it in Preset.

```C#
[Serializable]
[KnotTypeInfo(displayName: "My Custom Project Mod", MenuCustomName = "Test/My Custom Project Mod")]
public class MyCustomProjectMod : IKnotModAction
{
    public bool DoSomething = true;

    public string BuildDescription()
    {
        if (DoSomething)
            return "My Custom Project Mod will do something";

        return "My Custom Project Mod yet does nothing";
    }

    public IEnumerator Perform(EventHandler<IKnotModActionResult> onActionPerformed)
    {
        try
        {
            if (DoSomething)
                onActionPerformed?.Invoke(this, KnotModActionResult.Completed("My Custom Project Mod did something"));
            else onActionPerformed?.Invoke(this, KnotModActionResult.Failed("My Custom Project Mod did nothing"));
        }
        catch (Exception e)
        {
            //onActionPerformed should always be called even if it fails, otherwise the Mod chain will stall
            onActionPerformed?.Invoke(this, KnotModActionResult.Failed(e.Message));
        }

        yield break;
    }
}
```

