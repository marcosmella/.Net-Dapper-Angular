import { Component, Inject, OnInit } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { SafeUrl } from "@angular/platform-browser";

import { DragDropFileService } from "./../../../services/drag-drop-file.service";
import { ModalComponent } from "./../../../components/modal/modal.component";

@Component({
	selector: "app-modal-file-viewer",
	templateUrl: "./modal-file-viewer.component.html",
	styleUrls: ["./modal-file-viewer.component.scss"]
})
export class ModalFileViewerComponent implements OnInit {
	public protectedUrl: SafeUrl;
	public dataFile: string;

	constructor(
		@Inject(MAT_DIALOG_DATA) public data: any,
		public dialogRef: MatDialogRef<ModalComponent>,
		private dragDropFileService: DragDropFileService
	) {}

	ngOnInit(): void {
		if (this.data.item[0].dropFileType === "image") {
			this.showImage(this.data.item[0].file);
		} else {
			this.showFile(this.data.item[0].file);
		}
	}

	showImage(file: File): void {
		if (this.data.item[0].isUpdate) {
			const extension = this.dragDropFileService.getExtensionFile(this.data.item[0].file.url).split(".")[1];
			this.dragDropFileService.viewImage(this.data.item[0].file.url, {}, extension).then((url) => {
				this.dataFile = url;
			});
		} else {
			const reader = new FileReader();
			reader.onload = () => {
				const url = reader.result;
				const type = url.toString().split(",")[0];
				const data = url.toString().split(",")[1];
				const dataURL = `${type},${data}`;
				this.dataFile = dataURL;
			};
			reader.readAsDataURL(file);
		}
	}

	showFile(file: File): void {
		if (this.data.item[0].isUpdate) {
			const extension = this.dragDropFileService.getExtensionFile(this.data.item[0].file.url).split(".")[1];
			this.dragDropFileService.viewFile(this.data.item[0].file.url, {}, extension).then((url) => {
				this.protectedUrl = url;
			});
		} else {
			const reader = new FileReader();
			reader.onload = () => {
				const url = reader.result;

				this.protectedUrl = url;
			};
			reader.readAsDataURL(file);
		}
	}
}
