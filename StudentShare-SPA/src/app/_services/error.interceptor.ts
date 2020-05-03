import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(
    req: import('@angular/common/http').HttpRequest<any>,
    next: import('@angular/common/http').HttpHandler
  ): import('rxjs').Observable<import('@angular/common/http').HttpEvent<any>> {
    return next.handle(req).pipe(
        catchError(error => { // catches the error that has been returned via HttpRequest
            if (error.status === 401) {
                return throwError(error.statusText); // throwError throws it back to components
            }

            if (error instanceof HttpErrorResponse) {
                const applicationError = error.headers.get('Application-Error'); // get the exact error from generated header in API
                if (applicationError) {
                    return throwError(applicationError); // return this error
                }
                const serverError = error.error;
                let modalStateErrors = '';
                if (serverError.errors && typeof serverError.errors === 'object') { // does the server error = a type of object?
                    for (const key in serverError.errors) {
                        if (serverError.errors[key]) { // if the error has a key (for example the problem lies in the password field)
                            modalStateErrors += serverError.errors[key] + '\n'; // add this key to the modalStateErrors to return
                        }
                    }
                }
                return throwError(modalStateErrors || serverError || 'Server Error'); // if error exists details of the exact error returned
            }

        })
    );
  }
}

export const ErrorInterceptorProvider = { // register a new provider for use
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
};
