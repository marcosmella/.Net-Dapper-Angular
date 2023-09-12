import { Component, Inject, OnInit, ViewChild } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

import { ModalComponent } from "./../../../../components/modal/modal.component";
import { LoadingSpinnerComponent } from "./../../../../components/loading-spinner/loading-spinner.component";
import { EnumImageType } from "./../../../../models/file-type.model";

@Component({
	selector: "app-modal-image-preview",
	templateUrl: "./modal-image-preview.component.html",
	styleUrls: ["./modal-image-preview.component.scss"]
})
export class ModalImagePreviewComponent implements OnInit {
	@ViewChild("spinner", { static: true })
	spinner: LoadingSpinnerComponent;

	constructor(private dialogRef: MatDialogRef<ModalComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {}

	ngOnInit(): void {}

	isValidExtension(): boolean {
		return this.data.extension in EnumImageType;
	}

	cancel(): void {
		this.dialogRef.close();
	}
}
