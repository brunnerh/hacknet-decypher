# hacknet-decypher

Automatically decrypts `.dec` files from the game [Hacknet](http://hacknet-os.com/).

The basic algorithm can be found in the game in the form of C# code. This application uses that algorithm and brute-forces the decryption, so no password is needed.

Tools like [onlineocr.net](https://www.onlineocr.net/) can be used to more easily get the encrypted text out of the game. (Then again, the unencrypted file contents are stored in plain XML files in the game data directories, so none of this is really necessary.)

## `.dec` File Format

These files are broken into two lines of data, the first stores the headers, the second the content.

The header looks like this:

```plain
#DEC_ENC::<message>::<link>::<"ENCODED">[::<file-extension>]
```

| Header           | Description                                                                            |
| ---------------- | -------------------------------------------------------------------------------------- |
| `message`        | A subject/title line.                                                                  |
| `link`           | IP address of machine on which message was encrypted.                                  |
| `"ENCODED"`      | The text literal `"ENCODED"`.<br> Used to verify that the entered password is correct. |
| `file-extention` | Optional file extension of the decoded document                                        |

All of the above headers except the `"ENCODED"` literal are encrypted using a hard-coded seed. The code used for both the `"ENCODED"` header and the content are derived from the user-provided password.

That header makes a known-plaintext brute-force attack trivial.