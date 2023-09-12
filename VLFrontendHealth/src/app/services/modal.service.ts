import { Observable } from "rxjs/internal/Observable";
import { Injectable } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";

import { ModalComponent } from "../components/modal/modal.component";
import { DialogData } from "../models/dialog-data.model";

@Injectable({
	providedIn: "root"
})
export class ModalService {
	constructor(public dialog: MatDialog) {}

	openDialog(data: DialogData, width: string = "480px"): Observable<any> {
		const dialogRef = this.dialog.open(ModalComponent, {
			width: width ? width : null,
			data: {
				title: data.title,
				message: data.message,
				noButtonMessage: data.noButtonMessage,
				okButtonMessage: data.okButtonMessage,
				warningToast: data.warningToast,
				attentionToast: data.attentionToast
			}
		});
		return dialogRef.afterClosed();
	}
}
