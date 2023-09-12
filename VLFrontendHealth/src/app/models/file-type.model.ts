export class FileType {
	id: number;
	description: string;
}

export enum EnumFileType {
	"odt" = "file",
	"pdf" = "file",
	"doc" = "file",
	"docx" = "file",
	"jpg" = "image",
	"jpeg" = "image",
	"png" = "image",
	"bmp" = "image"
}

export enum EnumImageType {
	"jpg" = "image",
	"jpeg" = "image",
	"png" = "image",
	"bmp" = "image"
}

export enum FileIconType {
	"pdf" = "file-pdf",
	"jpg" = "file-jpg",
	"jpeg" = "file-jpg",
	"png" = "file-png"
}
