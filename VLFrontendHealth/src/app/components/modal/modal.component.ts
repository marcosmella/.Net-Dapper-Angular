import { Component, OnInit, Inject, AfterViewInit } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

import { DialogData } from "../../models/dialog-data.model";
import { AlertService } from "../../services/alert.service";

@Component({
	selector: "app-modal",
	templateUrl: "./modal.component.html",
	styleUrls: ["./modal.component.scss"]
})
export class ModalComponent implements OnInit, AfterViewInit {
	constructor(
		public dialogRef: MatDialogRef<ModalComponent>,
		@Inject(MAT_DIALOG_DATA) public data: DialogData,
		private alertService: AlertService
	) {}

	ngOnInit(): void {}

	ngAfterViewInit(): void {
		this.attentionToast();
		this.warningToast();
	}

	attentionToast(): void {
		if (this.data.attentionToast) {
			const messageAttention = Array<string>();
			messageAttention.push(this.data.attentionToast);
			this.alertService.warning(messageAttention, "attentionToast");
		}
	}

	warningToast(): void {
		if (this.data.warningToast) {
			const messageAtention = Array<string>();
			messageAtention.push(this.data.warningToast);
			this.alertService.error(messageAtention, "warningToast");
		}
	}

	onClose(): void {
		this.dialogRef.close();
	}

	onOkClick(): void {
		if (this.data.action) {
			this.data.action();
		}
		this.dialogRef.close(true);
	}
}
