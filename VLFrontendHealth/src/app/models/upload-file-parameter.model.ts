export interface UploadFileParameter {
	fileToUpload: File;
	entityTypeId: number;
	entityId: number;
	fileTypeId: number;
	fileType: string;
	active: boolean;
	fileDescription?: string;
}
