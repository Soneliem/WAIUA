# Localization
WAIUA has full localization support and any number of languages can be added and changed natively. By default the app changes it's language based on the system language.

## Adding Translations
There are two different ways of adding translations to this project. The first method is probably the easiest with the second being slightly complicated for those that have never done it before.

### Guidelines/Tips for both methods:
- Only add translations for things that you are 100% sure on. Partially translating files is ok as long as you put in some effort
- One of the keys named: "Translator" has an English translation of:
`Translated into English By Soneliem`
Change this to:
`Translated into [language-you-are-translating-to] By [your-name/username]`
If there is already a name there, simply add it to the end.
- Please try to only add generic languages such as English(en) rather than English-United States(en-US)

## Method 1: [POEditor](https://poeditor.com/join/project?hash=eDxQpoYC9q)
POEditor lets people collaboratively edit translations, supports auto-translations, quality checks, etc. It's very easy to use and let's me monitor/help people.
### Steps:
- Access the website [here](https://poeditor.com/join/project?hash=eDxQpoYC9q)
- Select the languages you want to help translate and press "Join Translation"
 - If it re-directs you to the home page click on "WAIUA"
 - You can add another language by requesting it (I will accept it as fast as I can)
- The first column is the English reference with the second column being what you can edit
 - You can change English to another language with the "Set Reference Language" button towards the top right
- Changes are auto saved
- After being proof-read, your changes will be pushed to the GitHub repo and included in the next release

## Method 2: Directly Adding .resx files
WPF apps like WAIUA use .resx (Microsoft .NET Managed Resource) files to handle localization. Method 1 simply creates one of these using a nice UI. These files can be opened/edited using Visual Studio/Code or a text editor if you really want.
### Steps:
- Git clone this repository
- Duplicate [this](https://github.com/Soneliem/WAIUA/blob/master/WAIUA/Properties/Resources.en-001.resx) English template or any of the [resource files](https://github.com/Soneliem/WAIUA/tree/master/WAIUA/Properties) in a Language of your choice.
- Change the ".en-001." to the corresponding Language tag in the table [here](https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c?redirectedfrom=MSDN).
- Edit the file with your translations and ensure it is in the [correct folder](https://github.com/Soneliem/WAIUA/tree/master/WAIUA/Properties)
- Create a pull request with the added file(s)

## Credits:
**German:**
- CemsA
- NNebus
**Arabic:**
- Chica
**French:**
- Rayjacker
**Japanese:**
- Aron
- nepixjp
**Russian:**
- ZzyzxFox
- DXGames
**Spanish:**
- ZzyzxFox
- FamiTom
